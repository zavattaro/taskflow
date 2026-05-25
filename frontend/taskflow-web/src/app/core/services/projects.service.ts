import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { Project } from '../../features/projects/project.model';
import { CreateProjectRequest } from '../../features/projects/create-project-request.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ProjectsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiBaseUrl}/Projects`;

  getAll(): Observable<Project[]> {
    return this.http.get<Project[]>(this.apiUrl).pipe(
      map(projects => projects),
      catchError(error => throwError(() => error))
    );
  }

  create(request: CreateProjectRequest): Observable<Project> {
    return this.http.post<Project>(this.apiUrl, request).pipe(
      map(project => project),
      catchError(error => throwError(() => error))
    );
  }
}