import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { TasksService } from './tasks.service';
import { TaskItem } from '../../features/tasks/task-item.model';

describe('TasksService', () => {
  let service: TasksService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TasksService],
    });

    service = TestBed.inject(TasksService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch tasks for a project', () => {
    const projectId = 'project-1';

    const mockTasks: TaskItem[] = [
      {
        id: '1',
        title: 'Task 1',
        description: null,
        status: 'Todo',
        projectId,
      },
      {
        id: '2',
        title: 'Task 2',
        description: 'Descrição',
        status: 'Doing',
        projectId,
      },
    ];

    service.getAll(projectId).subscribe(tasks => {
      expect(tasks.length).toBe(2);
      expect(tasks[0].title).toBe('Task 1');
      expect(tasks[1].status).toBe('Doing');
    });

    const req = httpMock.expectOne(req =>
      req.url.includes(`/projects/${projectId}/tasks`)
    );

    expect(req.request.method).toBe('GET');

    req.flush(mockTasks);
  });
});