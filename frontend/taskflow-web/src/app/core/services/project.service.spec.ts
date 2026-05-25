import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { ProjectsService } from './projects.service';
import { Project } from '../../features/projects/project.model';

describe('ProjectsService', () => {
  let service: ProjectsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProjectsService],
    });

    service = TestBed.inject(ProjectsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch projects list', () => {
    const mockProjects: Project[] = [
      { id: '1', name: 'Projeto 1', description: null },
      { id: '2', name: 'Projeto 2', description: 'Descrição' },
    ];

    service.getAll().subscribe(projects => {
      expect(projects.length).toBe(2);
      expect(projects[0].name).toBe('Projeto 1');
    });

    const req = httpMock.expectOne(req =>
      req.url.includes('/Projects')
    );

    expect(req.request.method).toBe('GET');

    req.flush(mockProjects);
  });
});