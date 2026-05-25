import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { TaskItem } from '../../features/tasks/task-item.model';
import { CreateTaskItemRequest } from '../../features/tasks/create-task-item-request.model';
import { UpdateTaskItemStatusRequest } from '../../features/tasks/update-task-item-status-request.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TasksService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = `${environment.apiBaseUrl}/projects`;

  getAll(projectId: string): Observable<TaskItem[]> {
    return this.http
      .get<TaskItem[]>(`${this.apiBaseUrl}/${projectId}/tasks`)
      .pipe(
        map(tasks => tasks),
        catchError(error => throwError(() => error))
      );
  }

  create(
    projectId: string,
    body: CreateTaskItemRequest
  ): Observable<TaskItem> {
    return this.http
      .post<TaskItem>(`${this.apiBaseUrl}/${projectId}/tasks`, body)
      .pipe(
        map(task => task),
        catchError(error => throwError(() => error))
      );
  }

  updateStatus(
    projectId: string,
    taskId: string,
    body: UpdateTaskItemStatusRequest
  ): Observable<TaskItem> {
    return this.http
      .patch<TaskItem>(
        `${this.apiBaseUrl}/${projectId}/tasks/${taskId}/status`,
        body
      )
      .pipe(
        map(task => task),
        catchError(error => throwError(() => error))
      );
  }
}