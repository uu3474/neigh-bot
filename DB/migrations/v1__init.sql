create table users (
	id bigint primary key,
	create_time timestamptz not null default now(),
	first_name text,
	last_name text,
	user_name text,
	phone text
);
create index users_user_name_idx ON users (user_name);
create index users_phone_idx ON users (phone);

create table reviews (
	id bigserial primary key,
	create_time timestamptz not null default now(),
	from_user bigint references users (id),
	to_user bigint references users (id),
	grade smallint not null,
	review text not null
);
create index reviews_create_time_idx ON reviews (create_time);
create index reviews_from_user_idx ON reviews (from_user);
create index reviews_to_user_idx ON reviews (to_user);