
CREATE OR REPLACE FUNCTION f_load_secretaries()
    RETURNS secretary
    LANGUAGE plpgsql
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $$
select * from secretary
$$;