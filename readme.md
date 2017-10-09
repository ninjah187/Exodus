# Exodus
## Simple database migrator for C#.

Exodus is opinionated database migration library for C#. It's main purpose is to make database migrating process as smooth as possible, so there's lot of convention over configuration.

Features:
- SQL script per migration approach
-- script file name format: [version] - [migration name].sql
e.g: 0042 - AnswerToEverythingAdded.sql
[version] - integer indicating database version number
[migration name] - string which briefly describes migration
- setup database before running migrations:
-- create database if not exists
-- drop and create database
