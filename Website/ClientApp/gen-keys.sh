#!/bin/bash

openssl req -x509 -newkey rsa:4096 -keyout keys/key.pem -out keys/cert.pem -days 365;
openssl pkcs12 -export -out keys/website.pfx -inkey keys/key.pem -in keys/cert.pem;
