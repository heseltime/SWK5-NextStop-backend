create table company
(
    company_id     serial
        primary key,
    name           varchar(255) not null,
    admin_username varchar(255) not null,
    admin_password varchar(255) not null
);

alter table company
    owner to postgres;

create table route
(
    route_id        serial
        primary key,
    route_number    varchar(255) not null,
    validity_period varchar(100),
    day_validity    varchar(50),
    company_id      integer
        references company
);

alter table route
    owner to postgres;

create table stop
(
    stop_id         serial
        primary key,
    name            varchar(255) not null,
    short_name      varchar(50),
    gps_coordinates varchar(50)
);

alter table stop
    owner to postgres;

create table schedule
(
    schedule_id    serial
        primary key,
    route_id       integer
        references route,
    date           date not null,
    time           time,
    connections    integer,
    validity_start date,
    validity_stop  date
);

alter table schedule
    owner to postgres;

create table check_in
(
    check_in_id serial
        primary key,
    schedule_id integer
        references schedule,
    route_id    integer
        references route,
    stop_id     integer
        references stop,
    date_time   timestamp    not null,
    api_key     varchar(255) not null
);

alter table check_in
    owner to postgres;

create table delay_info
(
    delay_id        serial
        primary key,
    schedule_id     integer
        references schedule,
    time_difference integer,
    updated_time    timestamp not null
);

alter table delay_info
    owner to postgres;

create table holiday
(
    holiday_id      serial
        primary key,
    date            date    not null,
    description     varchar(255),
    is_school_break boolean not null,
    company_id      integer
        references company
);

alter table holiday
    owner to postgres;

create table passenger
(
    passenger_id serial
        primary key,
    query        varchar(255),
    location     varchar(255)
);

alter table passenger
    owner to postgres;

create table info_screen
(
    screen_id        serial
        primary key,
    stop_id          integer
        references stop,
    date_time        timestamp not null,
    displayed_routes varchar(255)
);

alter table info_screen
    owner to postgres;

create table route_stop
(
    route_id        integer not null
        references route,
    stop_id         integer not null
        references stop,
    sequence_number integer,
    primary key (route_id, stop_id)
);

alter table route_stop
    owner to postgres;

create table route_stop_schedule
(
    route_stop_id   serial
        primary key,
    schedule_id     integer not null
        references schedule,
    stop_id         integer not null
        references stop,
    sequence_number integer not null,
    time            time    not null
);

alter table route_stop_schedule
    owner to postgres;

INSERT INTO public.company (company_id, name, admin_username, admin_password) VALUES (0, 'KHJ', 'jack.heseltine@khg.jku.at', 'admin');
INSERT INTO public.company (company_id, name, admin_username, admin_password) VALUES (1, 'Pastoral', 'khg@dioezese-linz.at', 'admin');


INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (2, 'KHG Foyer', 'Foyer', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (3, 'KHG Raum der Stille', 'Stille', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (4, 'KHG Kinderrestaurant', 'Kinderrestaurant', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (5, 'KHG TV-Raum', 'TV-Raum', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (6, 'KHG Zeitungsleseraum', 'Leseraum', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (7, 'KHG KHJ-Zimmer', 'KHJ-Zimmer', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (8, 'KHG Pastoralbüro', 'Pastoralbüro', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (9, 'KHG Garten', 'Garten', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (10, 'KHG Managementbüro', 'Managementbüro', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (11, 'KHG Kellerbar', 'Kellerbar', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (12, 'KHG 1', '1', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (13, 'KHG Fitnessraum', 'Fitnessraum', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (14, 'KHG Sauna', 'Sauna', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (15, 'KHG Billiardraum', 'Billiardraum', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (16, 'KHG Musikraum 1', 'Musik 1', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (17, 'KHG Musikraum 2', 'Musik 2', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (18, 'KHG Wäscheraum BT 1', 'Wäsche 1', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (19, 'KHG Wäscheraum BT 2', 'Wäsche 2', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (20, 'KHG Stiegenhaus BT 1 EG', 'Stiege 1 EG', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (21, 'KHG Stiegenhaus BT 1 1. Stock', 'Stiege 1 1. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (22, 'KHG Stiegenhaus BT 1 2. Stock', 'Stiege 1 2. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (23, 'KHG Stiegenhaus BT 1 3. Stock', 'Stiege 1 3. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (24, 'KHG Stiegenhaus BT 1 4. Stock', 'Stiege 1 4. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (25, 'KHG Stiegenhaus BT 1 5. Stock', 'Stiege 1 5. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (26, 'KHG Stiegenhaus BT 2 EG', 'Stiege 2 EG', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (27, 'KHG Stiegenhaus BT 2 1. Stock', 'Stiege 2 1. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (28, 'KHG Stiegenhaus BT 2 2. Stock', 'Stiege 2 2. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (29, 'KHG Stiegenhaus BT 2 3. Stock', 'Stiege 2 3. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (30, 'KHG Stiegenhaus BT 2 4. Stock', 'Stiege 2 4. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (31, 'KHG Stiegenhaus BT 2 5. Stock', 'Stiege 2 5. Stock', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (32, 'KHG 551', '551', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (43, 'KHG 553-1', 'KHG 553', '48.3358, 14.3173');
INSERT INTO public.stop (stop_id, name, short_name, gps_coordinates) VALUES (1, 'KHG Mensa', 'KHG Mensa', '48.3358, 14.3173');

INSERT INTO public.holiday (holiday_id, date, description, is_school_break, company_id) VALUES (2, '2024-12-25', 'Christmas Day', false, null);
INSERT INTO public.holiday (holiday_id, date, description, is_school_break, company_id) VALUES (3, '2024-04-01', 'Easter Monday', false, null);
INSERT INTO public.holiday (holiday_id, date, description, is_school_break, company_id) VALUES (4, '2025-01-01', 'New Year''s Day', false, null);
INSERT INTO public.holiday (holiday_id, date, description, is_school_break, company_id) VALUES (5, '2025-12-25', 'Christmas Day', false, null);
INSERT INTO public.holiday (holiday_id, date, description, is_school_break, company_id) VALUES (6, '2025-04-21', 'Easter Monday', false, null);
INSERT INTO public.holiday (holiday_id, date, description, is_school_break, company_id) VALUES (8, '2024-12-11', 'Test Holiday', true, null);
INSERT INTO public.holiday (holiday_id, date, description, is_school_break, company_id) VALUES (26, '2024-12-22', 'string1', true, 0);
INSERT INTO public.holiday (holiday_id, date, description, is_school_break, company_id) VALUES (1, '2024-01-01', 'New New Year''s Day', true, null);
INSERT INTO public.holiday (holiday_id, date, description, is_school_break, company_id) VALUES (29, '2025-02-14', 'Valentine''s', true, 0);

INSERT INTO public.route (route_id, route_number, validity_period, day_validity, company_id) VALUES (3, 'KHG Stiegenhaus BT 2 5. Stock zum Gottesdienst', '2024-01-01 to 2025-12-31', 'Weekends', 0);
INSERT INTO public.route (route_id, route_number, validity_period, day_validity, company_id) VALUES (2, 'Bierlauf BT 2', '2024-01-01 to 2025-12-31', 'Weekends', 0);
INSERT INTO public.route (route_id, route_number, validity_period, day_validity, company_id) VALUES (4, 'KHG Stiegenhaus BT 2 5. Stock zur Mensa', '2024-01-01 to 2025-12-31', 'Weekends', 0);
INSERT INTO public.route (route_id, route_number, validity_period, day_validity, company_id) VALUES (20, 'string1', 'string1', 'string', 0);
INSERT INTO public.route (route_id, route_number, validity_period, day_validity, company_id) VALUES (26, 'Test', 'Test', 'Test', 0);
INSERT INTO public.route (route_id, route_number, validity_period, day_validity, company_id) VALUES (1, 'Bierlauf BT 1', '2024-01-01 to 2025-12-31', 'Weekends', 0);

INSERT INTO public.schedule (schedule_id, route_id, date, time, connections, validity_start, validity_stop) VALUES (5, 1, '2025-01-01', null, null, '2024-12-31', '2025-12-31');
INSERT INTO public.schedule (schedule_id, route_id, date, time, connections, validity_start, validity_stop) VALUES (12, 1, '2025-01-02', null, null, '-infinity', '-infinity');

INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (3, 5, 21, 3, '23:55:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (7, 5, 25, 7, '23:58:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (6, 5, 24, 6, '23:56:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (5, 5, 23, 5, '23:59:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (4, 5, 22, 4, '23:59:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (2, 5, 20, 2, '23:57:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (80, 12, 20, 2, '15:44:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (81, 12, 21, 3, '15:45:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (82, 12, 22, 4, '15:46:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (83, 12, 23, 5, '15:47:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (84, 12, 24, 6, '15:48:00');
INSERT INTO public.route_stop_schedule (route_stop_id, schedule_id, stop_id, sequence_number, time) VALUES (85, 12, 25, 7, '15:49:00');
