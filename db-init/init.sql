SET client_encoding = 'UTF8';
SET statement_timeout = 0;
SET lock_timeout = 0;
-- SET idle_in_transaction_session_timeout = 0;
-- SET transaction_timeout = 0;
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

CREATE EXTENSION IF NOT EXISTS pg_trgm;

--


--
-- Name: reaction_type; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.reaction_type AS ENUM (
    'Like',
    'Dislike'
);


--
-- Name: calculate_course_average_rating(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calculate_course_average_rating() RETURNS trigger
    LANGUAGE plpgsql
    AS '
DECLARE
    -- Переменная для хранения ID курса, рейтинг которого нужно обновить
    course_to_update INT;
BEGIN
    -- Определяем ID курса, который требует обновления. 
    -- COALESCE используется для обработки INSERT (NEW) и DELETE (OLD).
    course_to_update := COALESCE(NEW.courseid, OLD.courseid);

    -- Шаг 1: Обновляем целевой курс (при INSERT, DELETE или UPDATE)
    UPDATE courses
    SET rating = (
        -- Пересчитываем среднее значение (AVG) всех оценок для этого курса
        -- AVG(reviews.review) вернет NULL, если обзоры удалены, что означает отсутствие рейтинга
        SELECT AVG(r.review)
        FROM reviews r
        WHERE r.courseid = course_to_update
    )
    WHERE id = course_to_update;

    -- Шаг 2: Обработка случая, когда при UPDATE изменился сам courseid обзора
    -- (Обзор перенесли с одного курса на другой)
    IF TG_OP = ''UPDATE'' AND OLD.courseid IS DISTINCT FROM NEW.courseid THEN
        -- Обновляем также и старый курс (откуда ушел обзор)
        UPDATE courses
        SET rating = (
            SELECT AVG(r.review)
            FROM reviews r
            WHERE r.courseid = OLD.courseid
        )
        WHERE id = OLD.courseid;
    END IF;

    -- AFTER-триггеры должны возвращать NULL
    RETURN NULL;
END;
';


--
-- Name: update_course_search_vector(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.update_course_search_vector() RETURNS trigger
    LANGUAGE plpgsql
    AS '
BEGIN
    NEW.searchvector := to_tsvector(''russian'',
        coalesce(NEW.name, '''') || '' '' ||
        coalesce(NEW.description, '''') || '' '' ||
        coalesce(
            (SELECT string_agg(c.Name, '' '')
             FROM course_categories cce
             JOIN Categories c ON c.Id = cce.categoryid
             WHERE cce.courseid = NEW.id),
            ''''
        )
    );
    RETURN NEW;
END;
';


--
-- Name: update_review_reaction_counts(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.update_review_reaction_counts() RETURNS trigger
    LANGUAGE plpgsql
    AS '
BEGIN
    -- 1. Обработка INSERT (Добавление новой реакции)
    IF (TG_OP = ''INSERT'') THEN
        IF NEW.reactiontype = ''Like'' THEN
            UPDATE reviews 
            SET likecount = likecount + 1 
            WHERE id = NEW.reviewid;
        ELSIF NEW.reactiontype = ''Dislike'' THEN
            UPDATE reviews 
            SET dislikecount = dislikecount + 1 
            WHERE id = NEW.reviewid;
        END IF;
        RETURN NEW;
        
    -- 2. Обработка DELETE (Удаление реакции)
    ELSIF (TG_OP = ''DELETE'') THEN
        -- OLD - содержит данные удаляемой строки
        IF OLD.reactiontype = ''Like'' THEN
            UPDATE reviews 
            SET likecount = likecount - 1 
            WHERE id = OLD.reviewid;
        ELSIF OLD.reactiontype = ''Dislike'' THEN
            UPDATE reviews 
            SET dislikecount = dislikecount - 1 
            WHERE id = OLD.reviewid;
        END IF;
        RETURN OLD;
        
    -- 3. Обработка UPDATE (Смена реакции, например, с Like на Dislike)
    ELSIF (TG_OP = ''UPDATE'') THEN
        -- Если тип реакции не изменился, выходим
        IF NEW.reactiontype = OLD.reactiontype THEN
            RETURN NEW;
        END IF;

        -- А. Сначала убираем старую реакцию (декремент)
        IF OLD.reactiontype = ''Like'' THEN
            UPDATE reviews 
            SET likecount = likecount - 1 
            WHERE id = NEW.reviewid;
        ELSIF OLD.reactiontype = ''Dislike'' THEN
            UPDATE reviews 
            SET dislikecount = dislikecount - 1 
            WHERE id = NEW.reviewid;
        END IF;

        -- Б. Затем добавляем новую реакцию (инкремент)
        IF NEW.reactiontype = ''Like'' THEN
            UPDATE reviews 
            SET likecount = likecount + 1 
            WHERE id = NEW.reviewid;
        ELSIF NEW.reactiontype = ''Dislike'' THEN
            UPDATE reviews 
            SET dislikecount = dislikecount + 1 
            WHERE id = NEW.reviewid;
        END IF;
        
        RETURN NEW;
    END IF;

    RETURN NULL;
END;
';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: categories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.categories (
    id integer NOT NULL,
    name character varying(100),
    parentid integer
);


--
-- Name: categories_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.categories_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: categories_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.categories_id_seq OWNED BY public.categories.id;


--
-- Name: chapter; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.chapter (
    id integer NOT NULL,
    courseid integer NOT NULL,
    "order" integer,
    name character varying(100) NOT NULL
);


--
-- Name: chapter_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.chapter_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: chapter_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.chapter_id_seq OWNED BY public.chapter.id;


--
-- Name: course_categories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.course_categories (
    courseid integer,
    categoryid integer
);


--
-- Name: courses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.courses (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(50),
    creatorid integer,
    createdat timestamp without time zone DEFAULT now() CONSTRAINT courses_created_at_not_null NOT NULL,
    field character varying(150) NOT NULL,
    searchvector tsvector,
    rating numeric(3,0) DEFAULT 0,
    imgfilekey character varying(500)
);


--
-- Name: courses_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.courses_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: courses_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.courses_id_seq OWNED BY public.courses.id;


--
-- Name: favorit; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.favorit (
    userid integer CONSTRAINT subscription_userid_not_null NOT NULL,
    courseid integer CONSTRAINT subscription_coursid_not_null NOT NULL,
    favoritdate timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: lesson; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.lesson (
    name character varying(100),
    chapterid integer NOT NULL,
    id integer NOT NULL,
    "order" integer NOT NULL,
    courseid integer NOT NULL,
    isvisible boolean DEFAULT true NOT NULL
);


--
-- Name: lesson_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.lesson_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: lesson_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.lesson_id_seq OWNED BY public.lesson.id;


--
-- Name: lessonfiles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.lessonfiles (
    id integer NOT NULL,
    lessonid integer NOT NULL,
    filekey character varying(500) CONSTRAINT lessonfiles_fileurl_not_null NOT NULL,
    filename character varying(100) NOT NULL,
    filetype character varying(50) NOT NULL,
    "order" integer DEFAULT 0,
    cloudstore boolean NOT NULL
);


--
-- Name: lessonfiles_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.lessonfiles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: lessonfiles_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.lessonfiles_id_seq OWNED BY public.lessonfiles.id;


--
-- Name: profiles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.profiles (
    userid integer NOT NULL,
    bio text,
    avatarkey character varying(500),
    sociallinks jsonb DEFAULT '{}'::jsonb NOT NULL
);


--
-- Name: reviewreaction; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.reviewreaction (
    id integer NOT NULL,
    reviewid integer NOT NULL,
    userid integer NOT NULL,
    reactiontype character varying(10) NOT NULL
);


--
-- Name: reviewreaction_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.reviewreaction_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: reviewreaction_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.reviewreaction_id_seq OWNED BY public.reviewreaction.id;


--
-- Name: reviews; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.reviews (
    id integer NOT NULL,
    userid integer NOT NULL,
    courseid integer CONSTRAINT reviews_courceid_not_null NOT NULL,
    content character varying(255) CONSTRAINT reviews_review_not_null NOT NULL,
    review integer CONSTRAINT reviews_review_not_null1 NOT NULL,
    createdat timestamp with time zone DEFAULT now(),
    likecount integer DEFAULT 0,
    dislikecount integer DEFAULT 0,
    lastchangedat timestamp with time zone,
    CONSTRAINT reviews_review_check CHECK (((review >= 0) AND (review <= 10)))
);


--
-- Name: reviews_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.reviews_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: reviews_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.reviews_id_seq OWNED BY public.reviews.id;


--
-- Name: roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.roles (
    id integer NOT NULL,
    name character varying(20) NOT NULL
);


--
-- Name: roles_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.roles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.roles_id_seq OWNED BY public.roles.id;


--
-- Name: subscription; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.subscription (
    followerid integer,
    followingid integer,
    followingdate timestamp without time zone DEFAULT now(),
    CONSTRAINT chk_no_self_follow CHECK ((followerid <> followingid))
);


--
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    email character varying(100) NOT NULL,
    isdelete boolean DEFAULT false,
    password text NOT NULL
);


--
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- Name: usersroles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.usersroles (
    userid integer NOT NULL,
    roleid integer NOT NULL
);


--
-- Name: categories id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.categories ALTER COLUMN id SET DEFAULT nextval('public.categories_id_seq'::regclass);


--
-- Name: chapter id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.chapter ALTER COLUMN id SET DEFAULT nextval('public.chapter_id_seq'::regclass);


--
-- Name: courses id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.courses ALTER COLUMN id SET DEFAULT nextval('public.courses_id_seq'::regclass);


--
-- Name: lesson id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson ALTER COLUMN id SET DEFAULT nextval('public.lesson_id_seq'::regclass);


--
-- Name: lessonfiles id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lessonfiles ALTER COLUMN id SET DEFAULT nextval('public.lessonfiles_id_seq'::regclass);


--
-- Name: reviewreaction id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviewreaction ALTER COLUMN id SET DEFAULT nextval('public.reviewreaction_id_seq'::regclass);


--
-- Name: reviews id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviews ALTER COLUMN id SET DEFAULT nextval('public.reviews_id_seq'::regclass);


--
-- Name: roles id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roles ALTER COLUMN id SET DEFAULT nextval('public.roles_id_seq'::regclass);


--
-- Name: users id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- Data for Name: categories; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.categories VALUES (1, 'Разработка', NULL);
INSERT INTO public.categories VALUES (2, 'Дизайн', NULL);
INSERT INTO public.categories VALUES (3, 'Бизнес', NULL);
INSERT INTO public.categories VALUES (4, 'Наука', NULL);
INSERT INTO public.categories VALUES (5, 'Языки и коммуникации', NULL);
INSERT INTO public.categories VALUES (6, 'Frontend', 1);
INSERT INTO public.categories VALUES (7, 'Backend', 1);
INSERT INTO public.categories VALUES (8, 'Мобильная разработка', 1);
INSERT INTO public.categories VALUES (9, 'DevOps & Infrastructure', 1);
INSERT INTO public.categories VALUES (10, 'Data Science', 1);
INSERT INTO public.categories VALUES (11, 'UI/UX Design', 2);
INSERT INTO public.categories VALUES (12, 'Графический дизайн', 2);
INSERT INTO public.categories VALUES (13, '3D & Animation', 2);
INSERT INTO public.categories VALUES (14, 'Digital Marketing', 3);
INSERT INTO public.categories VALUES (15, 'Менеджмент', 3);
INSERT INTO public.categories VALUES (16, 'Финансы', 3);
INSERT INTO public.categories VALUES (17, 'Предпринимательство', 3);
INSERT INTO public.categories VALUES (18, 'Математика', 4);
INSERT INTO public.categories VALUES (19, 'Физика', 4);
INSERT INTO public.categories VALUES (20, 'Биология', 4);
INSERT INTO public.categories VALUES (21, 'Химия', 4);
INSERT INTO public.categories VALUES (22, 'Английский', 5);
INSERT INTO public.categories VALUES (23, 'Немецкий', 5);
INSERT INTO public.categories VALUES (24, 'Испанский', 5);
INSERT INTO public.categories VALUES (25, 'Коммуникации', 5);
INSERT INTO public.categories VALUES (26, 'HTML & CSS', 6);
INSERT INTO public.categories VALUES (27, 'JavaScript', 6);
INSERT INTO public.categories VALUES (28, 'React', 6);
INSERT INTO public.categories VALUES (29, 'Vue.js', 6);
INSERT INTO public.categories VALUES (30, 'Angular', 6);
INSERT INTO public.categories VALUES (31, 'Node.js', 7);
INSERT INTO public.categories VALUES (32, 'Python', 7);
INSERT INTO public.categories VALUES (33, 'Java', 7);
INSERT INTO public.categories VALUES (34, 'C#', 7);
INSERT INTO public.categories VALUES (35, 'Ruby', 7);
INSERT INTO public.categories VALUES (36, 'Android', 8);
INSERT INTO public.categories VALUES (37, 'iOS', 8);
INSERT INTO public.categories VALUES (38, 'Flutter', 8);
INSERT INTO public.categories VALUES (39, 'React Native', 8);
INSERT INTO public.categories VALUES (40, 'Docker', 9);
INSERT INTO public.categories VALUES (41, 'Kubernetes', 9);
INSERT INTO public.categories VALUES (42, 'AWS', 9);
INSERT INTO public.categories VALUES (43, 'Azure', 9);
INSERT INTO public.categories VALUES (44, 'CI/CD', 9);
INSERT INTO public.categories VALUES (45, 'Machine Learning', 10);
INSERT INTO public.categories VALUES (46, 'Data Analysis', 10);
INSERT INTO public.categories VALUES (47, 'Big Data', 10);
INSERT INTO public.categories VALUES (48, 'SQL', 10);
INSERT INTO public.categories VALUES (49, 'NoSQL', 10);
INSERT INTO public.categories VALUES (50, 'Figma', 11);
INSERT INTO public.categories VALUES (51, 'Sketch', 11);
INSERT INTO public.categories VALUES (52, 'Adobe XD', 11);
INSERT INTO public.categories VALUES (53, 'Prototyping', 11);
INSERT INTO public.categories VALUES (54, 'User Research', 11);
INSERT INTO public.categories VALUES (55, 'Photoshop', 12);
INSERT INTO public.categories VALUES (56, 'Illustrator', 12);
INSERT INTO public.categories VALUES (57, 'InDesign', 12);
INSERT INTO public.categories VALUES (58, 'Blender', 13);
INSERT INTO public.categories VALUES (59, 'Maya', 13);
INSERT INTO public.categories VALUES (60, 'Cinema 4D', 13);
INSERT INTO public.categories VALUES (61, 'SEO', 14);
INSERT INTO public.categories VALUES (62, 'Content Marketing', 14);
INSERT INTO public.categories VALUES (63, 'Social Media Marketing', 14);
INSERT INTO public.categories VALUES (64, 'Email Marketing', 14);
INSERT INTO public.categories VALUES (65, 'Project Management', 15);
INSERT INTO public.categories VALUES (66, 'Agile', 15);
INSERT INTO public.categories VALUES (67, 'Scrum', 15);
INSERT INTO public.categories VALUES (68, 'Accounting', 16);
INSERT INTO public.categories VALUES (69, 'Investing', 16);
INSERT INTO public.categories VALUES (70, 'Cryptocurrency', 16);
INSERT INTO public.categories VALUES (71, 'Startup', 17);
INSERT INTO public.categories VALUES (72, 'Business Planning', 17);
INSERT INTO public.categories VALUES (73, 'E-commerce', 17);
INSERT INTO public.categories VALUES (74, 'Algebra', 18);
INSERT INTO public.categories VALUES (75, 'Geometry', 18);
INSERT INTO public.categories VALUES (76, 'Calculus', 18);
INSERT INTO public.categories VALUES (77, 'Mechanics', 19);
INSERT INTO public.categories VALUES (78, 'Electricity', 19);
INSERT INTO public.categories VALUES (79, 'Quantum Physics', 19);
INSERT INTO public.categories VALUES (80, 'Genetics', 20);
INSERT INTO public.categories VALUES (81, 'Ecology', 20);
INSERT INTO public.categories VALUES (82, 'Microbiology', 20);
INSERT INTO public.categories VALUES (83, 'Organic Chemistry', 21);
INSERT INTO public.categories VALUES (84, 'Inorganic Chemistry', 21);
INSERT INTO public.categories VALUES (85, 'Biochemistry', 21);
INSERT INTO public.categories VALUES (86, 'Beginner English', 22);
INSERT INTO public.categories VALUES (87, 'Business English', 22);
INSERT INTO public.categories VALUES (88, 'Conversational English', 22);
INSERT INTO public.categories VALUES (89, 'Beginner German', 23);
INSERT INTO public.categories VALUES (90, 'Advanced German', 23);
INSERT INTO public.categories VALUES (91, 'Beginner Spanish', 24);
INSERT INTO public.categories VALUES (92, 'Travel Spanish', 24);
INSERT INTO public.categories VALUES (93, 'Public Speaking', 25);
INSERT INTO public.categories VALUES (94, 'Negotiation', 25);
INSERT INTO public.categories VALUES (95, 'Leadership', 25);
INSERT INTO public.categories VALUES (220, 'CSS Grid', 26);
INSERT INTO public.categories VALUES (221, 'Flexbox', 26);
INSERT INTO public.categories VALUES (222, 'SASS', 26);
INSERT INTO public.categories VALUES (223, 'ES6', 27);
INSERT INTO public.categories VALUES (224, 'TypeScript', 27);
INSERT INTO public.categories VALUES (225, 'Webpack', 27);
INSERT INTO public.categories VALUES (226, 'React Hooks', 28);
INSERT INTO public.categories VALUES (227, 'Redux', 28);
INSERT INTO public.categories VALUES (228, 'Vuex', 29);
INSERT INTO public.categories VALUES (229, 'Angular Components', 30);
INSERT INTO public.categories VALUES (230, 'Express.js', 31);
INSERT INTO public.categories VALUES (231, 'Django', 32);
INSERT INTO public.categories VALUES (232, 'Spring Boot', 33);
INSERT INTO public.categories VALUES (233, 'ASP.NET Core', 34);
INSERT INTO public.categories VALUES (234, 'Rails', 35);
INSERT INTO public.categories VALUES (235, 'Kotlin', 36);
INSERT INTO public.categories VALUES (236, 'Swift', 37);
INSERT INTO public.categories VALUES (237, 'Dart', 38);
INSERT INTO public.categories VALUES (238, 'JavaScript for Mobile', 39);
INSERT INTO public.categories VALUES (239, 'Containerization', 40);
INSERT INTO public.categories VALUES (240, 'Orchestration', 41);
INSERT INTO public.categories VALUES (241, 'Cloud Computing', 42);
INSERT INTO public.categories VALUES (242, 'DevOps Tools', 43);
INSERT INTO public.categories VALUES (243, 'CI Pipelines', 44);
INSERT INTO public.categories VALUES (244, 'Neural Networks', 45);
INSERT INTO public.categories VALUES (245, 'Pandas', 46);
INSERT INTO public.categories VALUES (246, 'Hadoop', 47);
INSERT INTO public.categories VALUES (247, 'PostgreSQL', 48);
INSERT INTO public.categories VALUES (248, 'MongoDB', 49);
INSERT INTO public.categories VALUES (249, 'Prototyping Tools', 50);
INSERT INTO public.categories VALUES (250, 'User Testing', 51);
INSERT INTO public.categories VALUES (251, 'Wireframing', 52);
INSERT INTO public.categories VALUES (252, 'Digital Illustration', 53);
INSERT INTO public.categories VALUES (253, 'Typography', 54);
INSERT INTO public.categories VALUES (254, '3D Modeling', 55);
INSERT INTO public.categories VALUES (255, 'Animation Basics', 56);
INSERT INTO public.categories VALUES (256, 'SEO Advanced', 57);
INSERT INTO public.categories VALUES (257, 'Content Strategy', 58);
INSERT INTO public.categories VALUES (258, 'SMM Tools', 59);
INSERT INTO public.categories VALUES (259, 'Agile Scrum', 60);
INSERT INTO public.categories VALUES (260, 'Kanban', 61);
INSERT INTO public.categories VALUES (261, 'Financial Planning', 62);
INSERT INTO public.categories VALUES (262, 'Stock Market', 63);
INSERT INTO public.categories VALUES (263, 'Business Canvas', 64);
INSERT INTO public.categories VALUES (264, 'E-shop Setup', 65);
INSERT INTO public.categories VALUES (265, 'Linear Algebra', 66);
INSERT INTO public.categories VALUES (266, 'Trigonometry', 67);
INSERT INTO public.categories VALUES (267, 'Differential Equations', 68);
INSERT INTO public.categories VALUES (268, 'Thermodynamics', 69);
INSERT INTO public.categories VALUES (269, 'Electromagnetism', 70);
INSERT INTO public.categories VALUES (270, 'Molecular Biology', 71);
INSERT INTO public.categories VALUES (271, 'Ecosystems', 72);
INSERT INTO public.categories VALUES (272, 'Organic Reactions', 73);
INSERT INTO public.categories VALUES (273, 'English Grammar', 74);
INSERT INTO public.categories VALUES (274, 'English Vocabulary', 75);
INSERT INTO public.categories VALUES (275, 'German Verbs', 76);
INSERT INTO public.categories VALUES (276, 'Spanish Grammar', 77);
INSERT INTO public.categories VALUES (277, 'Public Speaking Tips', 78);
INSERT INTO public.categories VALUES (278, 'Negotiation Skills', 79);
INSERT INTO public.categories VALUES (279, 'CSS Animations', 26);
INSERT INTO public.categories VALUES (280, 'JavaScript Frameworks', 27);
INSERT INTO public.categories VALUES (281, 'Vue Composition API', 29);
INSERT INTO public.categories VALUES (282, 'Angular Services', 30);
INSERT INTO public.categories VALUES (283, 'Node Express', 31);
INSERT INTO public.categories VALUES (284, 'Python Flask', 32);
INSERT INTO public.categories VALUES (285, 'Java Spring', 33);
INSERT INTO public.categories VALUES (286, 'C# WPF', 34);
INSERT INTO public.categories VALUES (287, 'Ruby Sinatra', 35);
INSERT INTO public.categories VALUES (288, 'Android Jetpack', 36);
INSERT INTO public.categories VALUES (289, 'iOS SwiftUI', 37);
INSERT INTO public.categories VALUES (290, 'Flutter Widgets', 38);
INSERT INTO public.categories VALUES (291, 'React Navigation', 39);
INSERT INTO public.categories VALUES (292, 'Docker Compose', 40);
INSERT INTO public.categories VALUES (293, 'K8s Pods', 41);
INSERT INTO public.categories VALUES (294, 'AWS EC2', 42);
INSERT INTO public.categories VALUES (295, 'Azure VM', 43);
INSERT INTO public.categories VALUES (296, 'Jenkins', 44);
INSERT INTO public.categories VALUES (297, 'Deep Learning', 45);
INSERT INTO public.categories VALUES (298, 'NumPy', 46);
INSERT INTO public.categories VALUES (299, 'Spark', 47);
INSERT INTO public.categories VALUES (300, 'MySQL', 48);
INSERT INTO public.categories VALUES (301, 'Redis', 49);
INSERT INTO public.categories VALUES (302, 'Figma Plugins', 50);
INSERT INTO public.categories VALUES (303, 'A/B Testing', 51);
INSERT INTO public.categories VALUES (304, 'Adobe Illustrator', 53);
INSERT INTO public.categories VALUES (305, 'Blender Basics', 55);
INSERT INTO public.categories VALUES (306, 'SEO Tools', 57);
INSERT INTO public.categories VALUES (307, 'SMM Analytics', 59);
INSERT INTO public.categories VALUES (308, 'PMP Certification', 60);
INSERT INTO public.categories VALUES (309, 'Crypto Basics', 63);
INSERT INTO public.categories VALUES (310, 'Startup Funding', 64);
INSERT INTO public.categories VALUES (311, 'Shopify', 65);
INSERT INTO public.categories VALUES (312, 'Statistics', 66);
INSERT INTO public.categories VALUES (313, 'Physics Lab', 69);
INSERT INTO public.categories VALUES (314, 'Biology Lab', 71);
INSERT INTO public.categories VALUES (315, 'Chemistry Lab', 73);
INSERT INTO public.categories VALUES (316, 'English Speaking', 74);
INSERT INTO public.categories VALUES (317, 'German Conversation', 76);
INSERT INTO public.categories VALUES (318, 'Spanish Conversation', 77);
INSERT INTO public.categories VALUES (319, 'Presentation Skills', 78);
INSERT INTO public.categories VALUES (320, 'Conflict Resolution', 79);
INSERT INTO public.categories VALUES (321, 'CSS Modules', 26);
INSERT INTO public.categories VALUES (322, 'JS Promises', 27);
INSERT INTO public.categories VALUES (323, 'React Context', 28);
INSERT INTO public.categories VALUES (324, 'Vue Directives', 29);
INSERT INTO public.categories VALUES (325, 'Angular Directives', 30);
INSERT INTO public.categories VALUES (326, 'Node Middleware', 31);
INSERT INTO public.categories VALUES (327, 'Python Async', 32);
INSERT INTO public.categories VALUES (328, 'Java Lambdas', 33);
INSERT INTO public.categories VALUES (329, 'C# LINQ', 34);
INSERT INTO public.categories VALUES (330, 'Ruby Gems', 35);
INSERT INTO public.categories VALUES (331, 'Android Studio', 36);
INSERT INTO public.categories VALUES (332, 'Xcode', 37);
INSERT INTO public.categories VALUES (333, 'Flutter State', 38);
INSERT INTO public.categories VALUES (334, 'Docker Volumes', 40);
INSERT INTO public.categories VALUES (335, 'K8s Deployments', 41);
INSERT INTO public.categories VALUES (336, 'AWS S3', 42);
INSERT INTO public.categories VALUES (337, 'Azure Functions', 43);
INSERT INTO public.categories VALUES (338, 'GitHub Actions', 44);
INSERT INTO public.categories VALUES (339, 'AI Ethics', 45);
INSERT INTO public.categories VALUES (340, 'Matplotlib', 46);
INSERT INTO public.categories VALUES (341, 'Kafka', 47);
INSERT INTO public.categories VALUES (342, 'Oracle DB', 48);
INSERT INTO public.categories VALUES (343, 'Cassandra', 49);
INSERT INTO public.categories VALUES (344, 'Sketch App', 51);
INSERT INTO public.categories VALUES (345, 'Photoshop Brushes', 53);
INSERT INTO public.categories VALUES (346, 'Maya Rigging', 56);
INSERT INTO public.categories VALUES (347, 'Google Ads', 57);
INSERT INTO public.categories VALUES (348, 'Instagram Marketing', 59);
INSERT INTO public.categories VALUES (349, 'Lean Startup', 64);
INSERT INTO public.categories VALUES (350, 'Etsy Shop', 65);
INSERT INTO public.categories VALUES (351, 'Probability', 66);
INSERT INTO public.categories VALUES (352, 'Relativity', 70);
INSERT INTO public.categories VALUES (353, 'Evolution', 72);
INSERT INTO public.categories VALUES (354, 'Chemical Bonds', 73);
INSERT INTO public.categories VALUES (355, 'English Idioms', 75);
INSERT INTO public.categories VALUES (356, 'German Grammar', 76);
INSERT INTO public.categories VALUES (357, 'Spanish Verbs', 77);
INSERT INTO public.categories VALUES (358, 'Team Building', 79);


--
-- Data for Name: chapter; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.chapter VALUES (3, 10, 1, 'счет чурок');
INSERT INTO public.chapter VALUES (4, 10, 1, 'счет чурок');
INSERT INTO public.chapter VALUES (5, 10, 4, 'счет чурок');
INSERT INTO public.chapter VALUES (7, 10, 4, 'счет чурок');
INSERT INTO public.chapter VALUES (1, 10, 4, 'sd1');
INSERT INTO public.chapter VALUES (8, 18, 4, 'счет чурок');
INSERT INTO public.chapter VALUES (9, 18, 4, 'ооп');
INSERT INTO public.chapter VALUES (10, 18, 5, 'типы данных');
INSERT INTO public.chapter VALUES (11, 18, 4, 'sd1');
INSERT INTO public.chapter VALUES (13, 25, 1, 'Введение');
INSERT INTO public.chapter VALUES (18, 26, 1, 'Введение');


--
-- Data for Name: course_categories; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.course_categories VALUES (24, 34);
INSERT INTO public.course_categories VALUES (24, 233);
INSERT INTO public.course_categories VALUES (25, 6);
INSERT INTO public.course_categories VALUES (25, 34);
INSERT INTO public.course_categories VALUES (26, 32);
INSERT INTO public.course_categories VALUES (26, 284);


--
-- Data for Name: courses; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.courses VALUES (8, 'Химия', 'описание курса', 222, '2025-11-18 17:39:40.90211', 'точные науки', '''курс'':3 ''описан'':2 ''хим'':1', 5, NULL);
INSERT INTO public.courses VALUES (24, 'c# asp net', 'описание курса', 228, '2025-12-13 20:10:39.122335', 'программирование', '''asp'':2 ''asp.net'':7 ''c'':1,6 ''core'':8 ''net'':3 ''курс'':5 ''описан'':4', NULL, 'course-images/24/26.12.2025 18:28:43_623dca1c-6c73-4fcd-8ceb-ab08a0c33938_courseimg');
INSERT INTO public.courses VALUES (7, 'Математика', 'описание курса', 222, '2025-11-18 17:38:59.13168', 'точные науки', '''курс'':3 ''математик'':1 ''описан'':2', NULL, NULL);
INSERT INTO public.courses VALUES (26, 'основы python', 'будет изучено основы DJango, Flask, FastIp', 236, '-infinity', 'Flask, Django, FastApi', '''django'':6 ''fastip'':8 ''flask'':7,11 ''python'':2,9,10 ''изуч'':4 ''основ'':1,5', NULL, 'course-images/26/20.01.2026 20:46:15_39fe44d0-8ca9-4210-8aff-47d0172c6e51_courseimg');
INSERT INTO public.courses VALUES (9, 'Биология', 'описание курса', 222, '2025-11-18 17:39:47.709177', 'точные науки', '''биолог'':1 ''курс'':3 ''описан'':2', NULL, NULL);
INSERT INTO public.courses VALUES (10, 'Питорн нах', 'описание курса', 222, '2025-11-18 17:40:02.312815', 'программирование', '''курс'':4 ''нах'':2 ''описан'':3 ''питорн'':1', NULL, NULL);
INSERT INTO public.courses VALUES (17, 'Питорн нах', 'описание курса', 228, '2025-11-22 00:04:43.133896', 'программирование', '''курс'':4 ''нах'':2 ''описан'':3 ''питорн'':1', NULL, NULL);
INSERT INTO public.courses VALUES (19, 'c++', 'описание курса', 228, '2025-11-22 00:05:04.523732', 'программирование', '''c'':1 ''курс'':3 ''описан'':2', NULL, NULL);
INSERT INTO public.courses VALUES (25, 'основы фронтенда', 'будет изучено основы c#, css, js, react', 228, '-infinity', 'программирование, фронтенд', '''c'':6 ''css'':7 ''js'':8 ''react'':9 ''изуч'':4 ''основ'':1,5 ''фронтенд'':2', NULL, NULL);
INSERT INTO public.courses VALUES (18, 'c#', 'описание курса', 228, '2025-11-22 00:04:56.290253', 'программирование', '''c'':1 ''курс'':3 ''описан'':2', NULL, NULL);


--
-- Data for Name: favorit; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.favorit VALUES (228, 18, '2025-12-18 01:07:15.131812');


--
-- Data for Name: lesson; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.lesson VALUES ('Введение', 3, 3, 1, 10, false);
INSERT INTO public.lesson VALUES ('Введение', 1, 4, 1, 10, false);
INSERT INTO public.lesson VALUES ('Введение2', 3, 6, 2, 10, false);
INSERT INTO public.lesson VALUES ('Введение3', 3, 7, 3, 10, false);
INSERT INTO public.lesson VALUES ('первая тема', 3, 8, 4, 10, false);
INSERT INTO public.lesson VALUES ('html кончала', 13, 12, 1, 25, false);
INSERT INTO public.lesson VALUES ('вторая тема', 10, 10, 1, 18, false);
INSERT INTO public.lesson VALUES ('html кончала', 13, 11, 1, 25, true);
INSERT INTO public.lesson VALUES ('Теория', 18, 13, 1, 26, true);
INSERT INTO public.lesson VALUES ('1 тема', 18, 15, 1, 26, false);


--
-- Data for Name: lessonfiles; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.lessonfiles VALUES (11, 15, 'lessons/15/20.01.2026 23:43:00_17bfe537-d429-4ef5-b179-297832967dee_file', '"правильно"', '"docx"', 3, true);


--
-- Data for Name: profiles; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.profiles VALUES (235, NULL, NULL, '{}');
INSERT INTO public.profiles VALUES (236, NULL, NULL, '{}');


--
-- Data for Name: reviewreaction; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.reviewreaction VALUES (3, 5, 228, 'Dislike');


--
-- Data for Name: reviews; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.reviews VALUES (5, 228, 8, 'ну такой, средненький', 5, '-infinity', 0, 1, '-infinity');


--
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.roles VALUES (1, 'user');
INSERT INTO public.roles VALUES (2, 'admin');


--
-- Data for Name: subscription; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.subscription VALUES (228, 236, '-infinity');


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.users VALUES (228, 'test8', '@ma', false, 'Gq3fjSCsuh+q8Jc+aB/zpQ==:97wojhklGTT3HmA8vjNT8UoF5erebA1yBqARHAqPZPpxSoFzU7eahx7kgKusR8TVE5B2RTdRXzwPL8EphPVNRdsS3MMniKXhWdgvksWQr+zwuei6NPUg9Hb8XIJBkbLLf4PAi5I2kK/oUra6192SaZjBv8TT1B9RSdSCeeO2PIg=');
INSERT INTO public.users VALUES (231, 'kaka', 'kaka', false, 'zSQu3n5gOBjUfij4JP98gw==:l14Bh36czOXyeNNxR6LdtqbWQzzYyrjOoVmKVJ25fxNqsBTeAEDt+u2kSPLASYMOrzPR5lJR+WXt+L16IyhcpX8LZcUaeCiBNXt7tF5vxCULLfC1EAWgtGF4oG4lHvLJDtjfsVOGwXm1FeZZytsB6EZaX+/eCQdLYyh5OASwmEY=');
INSERT INTO public.users VALUES (222, 'test7', 'test7', false, 'Fs4/blNTW0JHUTeGU+Cuuw==:XknuYzLD9zXKFlhwJberM3Lpc0vEIBhW85h117+wMciJ5dj0xnCk5GofaSpbXdhdRkz/ea5eXuNltZdDXx6vpKztj6a/80retmtYH9B+fOHclQ7n8jfjy79qBsv/nn5Uc2haQwLFwlQckqWmMVF8AqfxcXW1gU/jouysG7FxUE0=');
INSERT INTO public.users VALUES (224, 'test8', 'test8', false, 'ThhBan1+jPYN8Pd93ZBGnQ==:Qp3ytGPiH6fpvqDdf+cb/PVEvQI1uniF6R+HwbUkBB8cjegqeUiitrZjdL9IjfR/BtD8dhLUOxvUrJZ0hOgyarKrSZL2w3UbtOJtTnblSTUuONqLrAA4sIg007RwUBmbJdgcl5uTVtEcA066Wng1GD5gpafc7sLEyQs92pK3nUo=');
INSERT INTO public.users VALUES (226, 'test8', 'test', false, 'nayDd+Bl+Ahy+fIzdLqMHg==:W7iO2oLd1m5IFUscKNjqo9yYKxSRRQBECf1/t7HurSUmObMybH3ZNfZDlrG/XTxKBJDJh7CElZamkd3q9eZGiJHmZfpQ36GBye0ov8O1vRxIn6nuhsvDm8EiTqkEunu3zESH74Lcr48Hlc47dzSPc4kp92A7rxKqwgKi14uUsXE=');
INSERT INTO public.users VALUES (235, 'kaka', 'kaka@', false, 'f/Jx6u3ReQ0jbOiR7gyZDw==:/lfPMsua5JTgRkjVn7T8S4avyyQTBF0JE6/C3DTXK9YaOXkK7gvZah9DKcypg1+ShLYkEOpUixk+dTkuddPUNahuQjgK61GJ6296o4W5kuX0nB7+BPK3KKG22TE8ypsN3O/sMGmSsr4kK+r/ovski3zjHlW05cm+hYMoEUvcl54=');
INSERT INTO public.users VALUES (236, 'username', 'suka@mail.ru', false, 'ABSMlL1dcTvHFf1zmldhjg==:vbhGSOOjUfaDkd9zM2+x7dAl//hM4zzBqt0/7MD2B6ApGsqQo5JnlHUIbNnxJDbhu0wjCCSNmneVqjGWX54JbNi6DAPGDxr62et+fE3bsXWPQiTfHwKB7zyyekpwuZxE+Ek6nAMmpHOV4VIv8mpQU5vs2cSjgnABVX9eAxLf4G4=');


--
-- Data for Name: usersroles; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.usersroles VALUES (222, 1);
INSERT INTO public.usersroles VALUES (224, 1);
INSERT INTO public.usersroles VALUES (226, 1);
INSERT INTO public.usersroles VALUES (228, 1);
INSERT INTO public.usersroles VALUES (231, 1);
INSERT INTO public.usersroles VALUES (235, 1);
INSERT INTO public.usersroles VALUES (236, 1);


--
-- Name: categories_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.categories_id_seq', 358, true);


--
-- Name: chapter_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.chapter_id_seq', 18, true);


--
-- Name: courses_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.courses_id_seq', 26, true);


--
-- Name: lesson_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.lesson_id_seq', 15, true);


--
-- Name: lessonfiles_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.lessonfiles_id_seq', 11, true);


--
-- Name: reviewreaction_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.reviewreaction_id_seq', 4, true);


--
-- Name: reviews_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.reviews_id_seq', 6, true);


--
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.roles_id_seq', 2, true);


--
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.users_id_seq', 236, true);


--
-- Name: categories categories_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.categories
    ADD CONSTRAINT categories_pkey PRIMARY KEY (id);


--
-- Name: chapter chapter_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.chapter
    ADD CONSTRAINT chapter_pkey PRIMARY KEY (id);


--
-- Name: courses courses_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT courses_pkey PRIMARY KEY (id);


--
-- Name: favorit favorit_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.favorit
    ADD CONSTRAINT favorit_pkey PRIMARY KEY (userid, courseid);


--
-- Name: lesson lesson_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson
    ADD CONSTRAINT lesson_pkey PRIMARY KEY (id);


--
-- Name: lessonfiles lessonfiles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lessonfiles
    ADD CONSTRAINT lessonfiles_pkey PRIMARY KEY (id);


--
-- Name: profiles profiles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profiles
    ADD CONSTRAINT profiles_pkey PRIMARY KEY (userid);


--
-- Name: reviewreaction reviewreaction_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviewreaction
    ADD CONSTRAINT reviewreaction_pkey PRIMARY KEY (id);


--
-- Name: reviewreaction reviewreaction_reviewid_userid_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviewreaction
    ADD CONSTRAINT reviewreaction_reviewid_userid_key UNIQUE (reviewid, userid);


--
-- Name: reviews reviews_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviews
    ADD CONSTRAINT reviews_pkey PRIMARY KEY (id);


--
-- Name: reviews reviews_userid_courceid_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviews
    ADD CONSTRAINT reviews_userid_courceid_key UNIQUE (userid, courseid);


--
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- Name: categories unic; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.categories
    ADD CONSTRAINT unic UNIQUE (name);


--
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: usersroles usersroles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usersroles
    ADD CONSTRAINT usersroles_pkey PRIMARY KEY (userid, roleid);


--
-- Name: idx_categories_name_trgm; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_categories_name_trgm ON public.categories USING gin (name public.gin_trgm_ops);


--
-- Name: ix_courses_searchvector; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_courses_searchvector ON public.courses USING gin (searchvector);


--
-- Name: ix_subscription_unique_pair; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX ix_subscription_unique_pair ON public.subscription USING btree (followerid, followingid);


--
-- Name: reviewreaction review_reaction_counts_trigger; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER review_reaction_counts_trigger AFTER INSERT OR DELETE OR UPDATE ON public.reviewreaction FOR EACH ROW EXECUTE FUNCTION public.update_review_reaction_counts();


--
-- Name: courses trg_update_course_search_vector; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_update_course_search_vector BEFORE INSERT OR UPDATE ON public.courses FOR EACH ROW EXECUTE FUNCTION public.update_course_search_vector();


--
-- Name: reviews update_course_rating_trigger; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER update_course_rating_trigger AFTER INSERT OR DELETE OR UPDATE ON public.reviews FOR EACH ROW EXECUTE FUNCTION public.calculate_course_average_rating();


--
-- Name: categories categories_parentid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.categories
    ADD CONSTRAINT categories_parentid_fkey FOREIGN KEY (parentid) REFERENCES public.categories(id);


--
-- Name: chapter chapter_courseid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.chapter
    ADD CONSTRAINT chapter_courseid_fkey FOREIGN KEY (courseid) REFERENCES public.courses(id) ON DELETE CASCADE;


--
-- Name: course_categories course_categories_categiryid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_categories
    ADD CONSTRAINT course_categories_categiryid_fkey FOREIGN KEY (categoryid) REFERENCES public.categories(id) ON DELETE CASCADE;


--
-- Name: course_categories course_categories_courseid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_categories
    ADD CONSTRAINT course_categories_courseid_fkey FOREIGN KEY (courseid) REFERENCES public.courses(id) ON DELETE CASCADE;


--
-- Name: courses fk_creator; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT fk_creator FOREIGN KEY (creatorid) REFERENCES public.users(id) ON DELETE SET NULL;


--
-- Name: profiles fk_profile_user; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profiles
    ADD CONSTRAINT fk_profile_user FOREIGN KEY (userid) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: lesson lesson_chapterid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson
    ADD CONSTRAINT lesson_chapterid_fkey FOREIGN KEY (chapterid) REFERENCES public.chapter(id) ON DELETE CASCADE;


--
-- Name: lesson lesson_courseid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson
    ADD CONSTRAINT lesson_courseid_fkey FOREIGN KEY (courseid) REFERENCES public.courses(id);


--
-- Name: lessonfiles lessonfiles_lessonid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lessonfiles
    ADD CONSTRAINT lessonfiles_lessonid_fkey FOREIGN KEY (lessonid) REFERENCES public.lesson(id) ON DELETE CASCADE;


--
-- Name: reviewreaction reviewreaction_reviewid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviewreaction
    ADD CONSTRAINT reviewreaction_reviewid_fkey FOREIGN KEY (reviewid) REFERENCES public.reviews(id) ON DELETE CASCADE;


--
-- Name: reviewreaction reviewreaction_userid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviewreaction
    ADD CONSTRAINT reviewreaction_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: reviews reviews_courceid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviews
    ADD CONSTRAINT reviews_courceid_fkey FOREIGN KEY (courseid) REFERENCES public.courses(id) ON DELETE CASCADE;


--
-- Name: reviews reviews_userid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reviews
    ADD CONSTRAINT reviews_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: favorit subscription_coursid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.favorit
    ADD CONSTRAINT subscription_coursid_fkey FOREIGN KEY (courseid) REFERENCES public.courses(id);


--
-- Name: subscription subscription_followerid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.subscription
    ADD CONSTRAINT subscription_followerid_fkey FOREIGN KEY (followerid) REFERENCES public.users(id);


--
-- Name: subscription subscription_followingid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.subscription
    ADD CONSTRAINT subscription_followingid_fkey FOREIGN KEY (followingid) REFERENCES public.users(id);


--
-- Name: favorit subscription_userid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.favorit
    ADD CONSTRAINT subscription_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(id);


--
-- Name: usersroles usersroles_roleid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usersroles
    ADD CONSTRAINT usersroles_roleid_fkey FOREIGN KEY (roleid) REFERENCES public.roles(id);


--
-- Name: usersroles usersroles_userid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usersroles
    ADD CONSTRAINT usersroles_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--



