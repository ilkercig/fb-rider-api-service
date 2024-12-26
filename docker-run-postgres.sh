#!/bin/bash

docker run --name postgres \
  -e POSTGRES_USER=fbrideradmin \
  -e POSTGRES_PASSWORD=1234 \
  -e POSTGRES_DB=fbrider \
  -p 5432:5432 \
  -v postgres-data:/var/lib/postgresql/data \
  -d postgres:latest
