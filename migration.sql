CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Reservas" (
    "Id" uuid NOT NULL,
    "NombreCliente" character varying(50) NOT NULL,
    "FechaReserva" timestamp with time zone NOT NULL,
    "NumeroComensales" integer NOT NULL,
    "Notas" character varying(50),
    CONSTRAINT "PK_Reservas" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250414084553_InitialMigration', '9.0.4');

COMMIT;

