CREATE Table category(
    id int primary key GENERATED ALWAYS AS IDENTITY,
    category_name varchar(50) NOT NULL
);