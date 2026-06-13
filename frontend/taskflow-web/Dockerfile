# Stage 1: build
FROM node:22-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm ci

ARG API_BASE_URL=http://localhost:8080/api
COPY . .
RUN sed -i "s|apiBaseUrl: '.*'|apiBaseUrl: '${API_BASE_URL}'|" src/environments/environment.ts
RUN npm run build

# Stage 2: serve
FROM nginx:1.27-alpine AS final

COPY --from=build /app/dist/taskflow-web/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
