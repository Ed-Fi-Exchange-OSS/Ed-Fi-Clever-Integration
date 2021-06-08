IF NOT EXISTS ( SELECT  *
                FROM    sys.schemas
                WHERE   name = N'onerosterv11' )
    EXEC('CREATE SCHEMA [onerosterv11]');
GO