require('dotenv').config();
const express = require('express');
const fs = require('fs');
const http = require('http');
const https = require('https');
const path = require('path');
const config = require('./config.json');

const app = express();

const pfx = fs.readFileSync(config.certificate);
const pass = process.env.PASSPHRASE;
const credentials = {
  pfx : pfx,
  passphrase : pass
};

const HTTP_PORT = config.httpPort || config.fallbackPort;
const HTTPS_PORT = config.httpsPort || config.fallbackPort + 1;

app.use((req, res, next) => {
  if (req.secure) {
    next();
    // might need to return here?
  } else {
    res.redirect('https://' + req.headers.host + req.url);
  }
});

app.use('/api/ping/', require('./routes/api/ping.js'));

// Set static website path
// TODO expand this to a full path
const BUILD_PATH = path.resolve("build/");
if (!BUILD_PATH)
  console.error("Error getting the build path.");

// ???
app.use(express.json());
app.use(express.urlencoded({extended : false}));

// Create HTTP/HTTPS servers and pass them as express middleware
http.createServer(app).listen(HTTP_PORT);
https.createServer(credentials, app).listen(HTTPS_PORT);

console.log(
    `Listening on http port ${HTTP_PORT}, and https port ${HTTPS_PORT}`);

// Serve static path (might want to remove this and render html pages)
app.use(express.static(BUILD_PATH));

app.get('/*',
        (_, res) => { res.sendFile(path.join(BUILD_PATH, "index.html")); });
