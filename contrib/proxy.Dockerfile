FROM nginx
RUN rm -rf /usr/share/nginx/*
COPY ../Website/wwwroot/ /usr/share/nginx/
COPY nginx.conf /etc/nginx/
