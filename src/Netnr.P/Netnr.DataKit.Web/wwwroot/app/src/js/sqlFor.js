import { sqlForSQLite } from './sqlForSQLite';
import { sqlForMySQL } from './sqlForMySQL';
import { sqlForOracle } from './sqlForOracle';
import { sqlForSQLServer } from './sqlForSQLServer';
import { sqlForPostgreSQL } from './sqlForPostgreSQL';

var sqlFor = {
    sqlForSQLite, sqlForMySQL, sqlForOracle, sqlForSQLServer, sqlForPostgreSQL,
}

export { sqlFor }