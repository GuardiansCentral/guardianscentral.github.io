require('dotenv').config();
const sql = require('mssql');

const server = process.env.SERVER_IP;
const user = process.env.DATABASE_USER_ID;
const password = process.env.DATABASE_PASSWORD;
const database = process.env.DATABASE_NAME;
const databasePortString = process.env.DATABASE_PORT;
const databasePort = parseInt(databasePortString,10)

var serverConfig = {  
    server: server,  
    authentication: {
        type: 'default',
        options: {
            userName: user, 
            password: password  
        }
    },
    options: {
        database: database,
        port: databasePort,
        encrypt: false,
        trustServerCertificate: true
    }
}; 

async function getWeeklyRotatorsJS(){
    try {
        const pool = await sql.connect(serverConfig);

        // Call GetRotatorList stored procedure
        const rotatorListRequest = pool.request();
        const getAcvtiveWeeklyRotators = await rotatorListRequest.input('Name', sql.NVarChar, 'ActiveWeeklyRotators').execute('GetRotatorList');
        const rotatorListString = getAcvtiveWeeklyRotators.recordset[0].RotatorList;
        const rotatorHashList = JSON.parse(rotatorListString).map(String);

        // Call GetWeeklyRotators stored procedure
        const weeklyRotatorsRequest = pool.request();
        for (let i = 0; i < rotatorHashList.length; i++) {
            weeklyRotatorsRequest.input(`Rotator${i + 1}`, sql.BigInt, rotatorHashList[i]);
            console.log(rotatorHashList[i]);
        }

        const result = await weeklyRotatorsRequest.execute('GetWeeklyRotators');
        await pool.close();
        return result.recordset;
    
      } catch (err) {
        // Handle errors
        console.error('Error:', err);
      }
}
module.exports = getWeeklyRotatorsJS;
//Named with a JS prefix to differentiate it from the file in C#
