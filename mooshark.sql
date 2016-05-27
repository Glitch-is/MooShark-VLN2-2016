-- Database generated with pgModeler (PostgreSQL Database Modeler).
-- pgModeler  version: 0.8.2-beta1
-- PostgreSQL version: 9.5
-- Project Site: pgmodeler.com.br
-- Model Author: ---

-- object: mooshark | type: ROLE --
-- DROP ROLE IF EXISTS mooshark;
-- CREATE ROLE mooshark WITH ;
-- ddl-end --


-- Database creation must be done outside an multicommand file.
-- These commands were put in this file only for convenience.
-- -- object: mooshark | type: DATABASE --
-- -- DROP DATABASE IF EXISTS mooshark;
-- CREATE DATABASE mooshark
-- 	OWNER = mooshark
-- ;
-- -- ddl-end --
--

-- object: user | type: TABLE --
-- DROP TABLE IF EXISTS user CASCADE;
CREATE TABLE user(
	id int AUTO_INCREMENT NOT NULL,
	name varchar(75),
	email varchar(150),
	password varchar(257) NOT NULL,
	roleId integer NOT NULL,
	imagePath text,
	CONSTRAINT userPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: roles | type: TABLE --
-- DROP TABLE IF EXISTS roles CASCADE;
CREATE TABLE roles(
	id int AUTO_INCREMENT NOT NULL,
	name varchar(40) NOT NULL,
	CONSTRAINT rolePK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: courses | type: TABLE --
-- DROP TABLE IF EXISTS courses CASCADE;
CREATE TABLE courses(
	id int AUTO_INCREMENT NOT NULL,
	name varchar(100) NOT NULL,
	description text,
	CONSTRAINT coursesPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: teachers | type: TABLE --
-- DROP TABLE IF EXISTS teachers CASCADE;
CREATE TABLE teachers(
	id int AUTO_INCREMENT NOT NULL,
	userId integer NOT NULL,
	courseId integer NOT NULL,
	CONSTRAINT teachersPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: assignment | type: TABLE --
-- DROP TABLE IF EXISTS assignment CASCADE;
CREATE TABLE assignment(
	id int AUTO_INCREMENT NOT NULL,
	name varchar(50) NOT NULL,
	description text,
	deadline timestamp NOT NULL,
	courseId integer NOT NULL,
	CONSTRAINT assigmentPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: problem | type: TABLE --
-- DROP TABLE IF EXISTS problem CASCADE;
CREATE TABLE problem(
	id int AUTO_INCREMENT NOT NULL,
	name varchar(50) NOT NULL,
	description text NOT NULL,
	weight integer,
	sampleInput text NOT NULL,
	sampleOutput text NOT NULL,
	CONSTRAINT problemPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: testcase | type: TABLE --
-- DROP TABLE IF EXISTS testcase CASCADE;
CREATE TABLE testcase(
	id int AUTO_INCREMENT NOT NULL,
	input text NOT NULL,
	output text NOT NULL,
	CONSTRAINT testcasePK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: testcases | type: TABLE --
-- DROP TABLE IF EXISTS testcases CASCADE;
CREATE TABLE testcases(
	id int AUTO_INCREMENT NOT NULL,
	testcaseId integer NOT NULL,
	problemId integer NOT NULL,
	CONSTRAINT testcasesPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: problems | type: TABLE --
-- DROP TABLE IF EXISTS problems CASCADE;
CREATE TABLE problems(
	id int AUTO_INCREMENT NOT NULL,
	problemId integer NOT NULL,
	assignmentId integer NOT NULL,
	CONSTRAINT problemsPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: group | type: TABLE --
-- DROP TABLE IF EXISTS group CASCADE;
CREATE TABLE group(
	id int AUTO_INCREMENT NOT NULL,
	name varchar(50),
	CONSTRAINT groupPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: groups | type: TABLE --
-- DROP TABLE IF EXISTS groups CASCADE;
CREATE TABLE groups(
	id int AUTO_INCREMENT NOT NULL,
	userId integer NOT NULL,
	assignmentId integer NOT NULL,
	CONSTRAINT groupsPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: submission | type: TABLE --
-- DROP TABLE IF EXISTS submission CASCADE;
CREATE TABLE submission(
	id int AUTO_INCREMENT NOT NULL,
	code text NOT NULL,
	CONSTRAINT submissionPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: submissions | type: TABLE --
-- DROP TABLE IF EXISTS submissions CASCADE;
CREATE TABLE submissions(
	id int AUTO_INCREMENT NOT NULL,
	submissionId integer NOT NULL,
	groupId integer NOT NULL,
	CONSTRAINT submissionsPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: groupMembers | type: TABLE --
-- DROP TABLE IF EXISTS groupMembers CASCADE;
CREATE TABLE groupMembers(
	id int AUTO_INCREMENT NOT NULL,
	groupId integer NOT NULL,
	userId integer NOT NULL,
	CONSTRAINT groupMembersPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: clarification | type: TABLE --
-- DROP TABLE IF EXISTS clarification CASCADE;
CREATE TABLE clarification(
	id int AUTO_INCREMENT NOT NULL,
	authorId integer NOT NULL,
	message text NOT NULL,
	posted timestamp NOT NULL DEFAULT NOW()::timestamp,
	CONSTRAINT clarificationPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: clarificationAnswer | type: TABLE --
-- DROP TABLE IF EXISTS clarificationAnswer CASCADE;
CREATE TABLE clarificationAnswer(
	id int AUTO_INCREMENT NOT NULL,
	authorId integer NOT NULL,
	message smallint NOT NULL,
	posted timestamp NOT NULL DEFAULT NOW()::timestamp,
	clarificationId integer NOT NULL,
	CONSTRAINT clarificationAnswerPK PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: clarifications | type: TABLE --
-- DROP TABLE IF EXISTS clarifications CASCADE;
CREATE TABLE clarifications(
	id int AUTO_INCREMENT NOT NULL,
	clarificationId integer NOT NULL,
	problemId integer NOT NULL,
	CONSTRAINT clarificationsId PRIMARY KEY (id)

);
-- ddl-end --

-- ddl-end --

-- object: roleFK | type: CONSTRAINT --
-- ALTER TABLE user DROP CONSTRAINT IF EXISTS roleFK CASCADE;
ALTER TABLE user ADD CONSTRAINT roleFK FOREIGN KEY (roleId)
REFERENCES roles (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: teacherFK | type: CONSTRAINT --
-- ALTER TABLE teachers DROP CONSTRAINT IF EXISTS teacherFK CASCADE;
ALTER TABLE teachers ADD CONSTRAINT teacherFK FOREIGN KEY (userId)
REFERENCES user (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: courseFK | type: CONSTRAINT --
-- ALTER TABLE teachers DROP CONSTRAINT IF EXISTS courseFK CASCADE;
ALTER TABLE teachers ADD CONSTRAINT courseFK FOREIGN KEY (courseId)
REFERENCES courses (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: assignmentFK | type: CONSTRAINT --
-- ALTER TABLE assignment DROP CONSTRAINT IF EXISTS assignmentFK CASCADE;
ALTER TABLE assignment ADD CONSTRAINT assignmentFK FOREIGN KEY (courseId)
REFERENCES courses (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: testcaseFK | type: CONSTRAINT --
-- ALTER TABLE testcases DROP CONSTRAINT IF EXISTS testcaseFK CASCADE;
ALTER TABLE testcases ADD CONSTRAINT testcaseFK FOREIGN KEY (testcaseId)
REFERENCES testcase (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: problemFK | type: CONSTRAINT --
-- ALTER TABLE testcases DROP CONSTRAINT IF EXISTS problemFK CASCADE;
ALTER TABLE testcases ADD CONSTRAINT problemFK FOREIGN KEY (problemId)
REFERENCES problem (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: problemFK | type: CONSTRAINT --
-- ALTER TABLE problems DROP CONSTRAINT IF EXISTS problemFK CASCADE;
ALTER TABLE problems ADD CONSTRAINT problemFK FOREIGN KEY (problemId)
REFERENCES problem (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: assignmentFK | type: CONSTRAINT --
-- ALTER TABLE problems DROP CONSTRAINT IF EXISTS assignmentFK CASCADE;
ALTER TABLE problems ADD CONSTRAINT assignmentFK FOREIGN KEY (assignmentId)
REFERENCES assignment (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: groupFK | type: CONSTRAINT --
-- ALTER TABLE groups DROP CONSTRAINT IF EXISTS groupFK CASCADE;
ALTER TABLE groups ADD CONSTRAINT groupFK FOREIGN KEY (userId)
REFERENCES group (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: assignmentFK | type: CONSTRAINT --
-- ALTER TABLE groups DROP CONSTRAINT IF EXISTS assignmentFK CASCADE;
ALTER TABLE groups ADD CONSTRAINT assignmentFK FOREIGN KEY (assignmentId)
REFERENCES assignment (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: submissionFK | type: CONSTRAINT --
-- ALTER TABLE submissions DROP CONSTRAINT IF EXISTS submissionFK CASCADE;
ALTER TABLE submissions ADD CONSTRAINT submissionFK FOREIGN KEY (submissionId)
REFERENCES submission (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: groupFK | type: CONSTRAINT --
-- ALTER TABLE submissions DROP CONSTRAINT IF EXISTS groupFK CASCADE;
ALTER TABLE submissions ADD CONSTRAINT groupFK FOREIGN KEY (groupId)
REFERENCES group (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: groupFK | type: CONSTRAINT --
-- ALTER TABLE groupMembers DROP CONSTRAINT IF EXISTS groupFK CASCADE;
ALTER TABLE groupMembers ADD CONSTRAINT groupFK FOREIGN KEY (groupId)
REFERENCES group (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: userFK | type: CONSTRAINT --
-- ALTER TABLE groupMembers DROP CONSTRAINT IF EXISTS userFK CASCADE;
ALTER TABLE groupMembers ADD CONSTRAINT userFK FOREIGN KEY (userId)
REFERENCES user (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: authorFK | type: CONSTRAINT --
-- ALTER TABLE clarification DROP CONSTRAINT IF EXISTS authorFK CASCADE;
ALTER TABLE clarification ADD CONSTRAINT authorFK FOREIGN KEY (authorId)
REFERENCES user (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: authorFK | type: CONSTRAINT --
-- ALTER TABLE clarificationAnswer DROP CONSTRAINT IF EXISTS authorFK CASCADE;
ALTER TABLE clarificationAnswer ADD CONSTRAINT authorFK FOREIGN KEY (authorId)
REFERENCES user (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: clarificationFK | type: CONSTRAINT --
-- ALTER TABLE clarificationAnswer DROP CONSTRAINT IF EXISTS clarificationFK CASCADE;
ALTER TABLE clarificationAnswer ADD CONSTRAINT clarificationFK FOREIGN KEY (clarificationId)
REFERENCES clarification (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: clarificationFK | type: CONSTRAINT --
-- ALTER TABLE clarifications DROP CONSTRAINT IF EXISTS clarificationFK CASCADE;
ALTER TABLE clarifications ADD CONSTRAINT clarificationFK FOREIGN KEY (clarificationId)
REFERENCES clarification (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --

-- object: problemFK | type: CONSTRAINT --
-- ALTER TABLE clarifications DROP CONSTRAINT IF EXISTS problemFK CASCADE;
ALTER TABLE clarifications ADD CONSTRAINT problemFK FOREIGN KEY (problemId)
REFERENCES problem (id) MATCH FULL
ON DELETE NO ACTION ON UPDATE NO ACTION;
-- ddl-end --
