import { NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TasksService } from '../../core/services/tasks.service';
import { CreateTaskItemRequest } from './create-task-item-request.model';
import { TaskItem } from './task-item.model';
import { UpdateTaskItemStatusRequest } from './update-task-item-status-request.model';
import { AppHeaderComponent } from '../../shared/components/app-header/app-header.component';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [NgIf, NgFor, RouterLink, ReactiveFormsModule, AppHeaderComponent],
  template: `
  <app-header></app-header>
    <main style="padding: 24px; font-family: Arial, sans-serif;">
      <p>
        <a routerLink="/projects">← Voltar para projetos</a>
      </p>

      <h1>Tasks</h1>

      <form
        [formGroup]="createForm"
        (ngSubmit)="createTask()"
        style="
          margin-bottom: 24px;
          padding: 16px;
          border: 1px solid #ddd;
          border-radius: 8px;
          max-width: 500px;
        "
      >
        <div style="margin-bottom: 12px;">
          <label for="title" style="display: block; margin-bottom: 4px;">
            Título
          </label>
          <input
            id="title"
            type="text"
            formControlName="title"
            style="width: 100%; padding: 8px; box-sizing: border-box;"
          />

          <small
            *ngIf="createForm.controls.title.invalid && createForm.controls.title.touched"
            style="color: red;"
          >
            O título é obrigatório.
          </small>
        </div>

        <div style="margin-bottom: 12px;">
          <label for="description" style="display: block; margin-bottom: 4px;">
            Descrição
          </label>
          <textarea
            id="description"
            rows="3"
            formControlName="description"
            style="width: 100%; padding: 8px; box-sizing: border-box;"
          ></textarea>
        </div>

        <button type="submit" [disabled]="isSubmitting" style="padding: 8px 12px;">
          {{ isSubmitting ? 'Salvando...' : 'Criar task' }}
        </button>

        <p *ngIf="submitErrorMessage" style="color: red; margin-top: 12px;">
          {{ submitErrorMessage }}
        </p>
      </form>

      <p *ngIf="loading">Carregando tasks...</p>

      <p *ngIf="errorMessage" style="color: red;">
        {{ errorMessage }}
      </p>

      <p *ngIf="!loading && !errorMessage && tasks.length === 0">
        Nenhuma task encontrada.
      </p>

      <ul *ngIf="!loading && tasks.length > 0" style="padding-left: 20px;">
        <li *ngFor="let task of tasks" style="margin-bottom: 12px;">
          <strong>{{ task.title }}</strong>
          <br />
          <span>{{ task.description || 'Sem descrição.' }}</span>
          <br />
          <small>Status atual: {{ task.status }}</small>
          <br /><br />

          <label [for]="'status-' + task.id">Status:</label>
          <select
            [id]="'status-' + task.id"
            [value]="selectedStatuses[task.id] ?? task.status"
            (change)="setSelectedStatus(task.id, getSelectValue($event))"
            [disabled]="updatingTaskId === task.id"
            style="margin-left: 8px; padding: 4px;"
          >
            <option value="Todo">To Do</option>
            <option value="Doing">Doing</option>
            <option value="Done">Done</option>
          </select>

          <button
            type="button"
            (click)="saveTaskStatus(task)"
            [disabled]="updatingTaskId === task.id"
            style="margin-left: 8px; padding: 4px 8px;"
          >
            Salvar status
          </button>

          <small *ngIf="updatingTaskId === task.id" style="margin-left: 8px;">
            Atualizando...
          </small>
        </li>
      </ul>
    </main>
  `,
})
export class TasksComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly tasksService = inject(TasksService);
  private readonly fb = inject(FormBuilder);

  projectId = '';
  loading = true;
  isSubmitting = false;
  updatingTaskId: string | null = null;
  errorMessage = '';
  submitErrorMessage = '';
  tasks: TaskItem[] = [];

  readonly createForm = this.fb.group({
    title: ['', [Validators.required]],
    description: [''],
  });

  ngOnInit(): void {
    const projectId = this.route.snapshot.paramMap.get('projectId');

    if (!projectId) {
      this.errorMessage = 'Projeto inválido.';
      this.loading = false;
      return;
    }

    this.projectId = projectId;
    this.loadTasks();
  }

  loadTasks(): void {
    this.loading = true;
    this.errorMessage = '';

    this.tasksService.getAll(this.projectId).subscribe({
      next: (tasks) => {
        this.tasks = tasks;

        this.selectedStatuses = {};
        for (const task of tasks) {
          this.selectedStatuses[task.id] = task.status;
        }

        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar tasks.';
        this.loading = false;
      },
    });
  }

  createTask(): void {
    if (!this.projectId) {
      this.submitErrorMessage = 'Projeto inválido.';
      return;
    }

    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const title = this.createForm.controls.title.value?.trim() ?? '';
    const description = this.createForm.controls.description.value?.trim() ?? '';

    const request: CreateTaskItemRequest = description
      ? { title, description }
      : { title };

    this.isSubmitting = true;
    this.submitErrorMessage = '';

    this.tasksService.create(this.projectId, request).subscribe({
      next: () => {
        this.createForm.reset();
        this.loadTasks();
      },
      error: () => {
        this.submitErrorMessage = 'Erro ao criar task.';
        this.isSubmitting = false;
      },
      complete: () => {
        this.isSubmitting = false;
      },
    });
  }

  updateTaskStatus(task: TaskItem, status: string): void {
    if (!this.projectId || task.status === status) {
      return;
    }

    const request: UpdateTaskItemStatusRequest = { status };

    this.updatingTaskId = task.id;
    this.errorMessage = '';

    this.tasksService.updateStatus(this.projectId, task.id, request).subscribe({
      next: (updatedTask) => {
        this.tasks = this.tasks.map((item) =>
          item.id === updatedTask.id ? updatedTask : item
        );
      },
      error: () => {
        this.errorMessage = 'Erro ao atualizar status da task.';
      },
      complete: () => {
        this.updatingTaskId = null;
      },
    });
  }

  getSelectValue(event: Event): string {
    return (event.target as HTMLSelectElement).value;
  }

  selectedStatuses: Record<string, string> = {};

  setSelectedStatus(taskId: string, status: string): void {
  this.selectedStatuses[taskId] = status;
}

  saveTaskStatus(task: TaskItem): void {
    if (!this.projectId) {
      return;
    }

    const status = this.selectedStatuses[task.id] ?? task.status;
    const request: UpdateTaskItemStatusRequest = { status };

    this.updatingTaskId = task.id;
    this.errorMessage = '';

    this.tasksService.updateStatus(this.projectId, task.id, request).subscribe({
      next: (updatedTask) => {
        this.tasks = this.tasks.map((item) =>
          item.id === updatedTask.id ? updatedTask : item
      );

        this.selectedStatuses[task.id] = updatedTask.status;
      },
      error: () => {
        this.errorMessage = 'Erro ao atualizar status da task.';
      },
      complete: () => {
        this.updatingTaskId = null;
      },
    });
  }
}