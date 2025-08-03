#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE DATABASE identity_service;
    CREATE DATABASE contract_service;
    CREATE DATABASE document_service;
    CREATE DATABASE notification_service;
    CREATE DATABASE audit_service;
    
    GRANT ALL PRIVILEGES ON DATABASE identity_service TO $POSTGRES_USER;
    GRANT ALL PRIVILEGES ON DATABASE contract_service TO $POSTGRES_USER;
    GRANT ALL PRIVILEGES ON DATABASE document_service TO $POSTGRES_USER;
    GRANT ALL PRIVILEGES ON DATABASE notification_service TO $POSTGRES_USER;
    GRANT ALL PRIVILEGES ON DATABASE audit_service TO $POSTGRES_USER;
EOSQL