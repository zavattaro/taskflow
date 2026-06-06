import { NgClass, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TasksService } from '../../core/services/tasks.service';
import { CreateTaskItemRequest } from './create-task-item-request.model';
import { TaskItem } from './task-item.model';
import { UpdateTaskItemStatusRequest } from './update-task-item-status-request.model';
import { ErrorMessageComponent } from '../../shared/components/error-message/error-message.component';
import { AppButtonComponent } from '../../shared/components/app-button/app-button.component';
import { AppLoadingComponent } from '../../shared/components/app-loading/app-loading.component';
import { AppEmptyStateComponent } from '../../shared/components/app-empty-state/app-empty-state.component';
@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [NgIf, NgFor, NgClass, RouterLink, ReactiveFormsModule, ErrorMessageComponent, AppButtonComponent, AppLoadingComponent, AppEmptyStateComponent],
  template: `
    <main class="tasks-container">
      <a routerLink="/projects" class="tasks-back">← Voltar para projetos</a>

      <div class="tasks-header">
        <h1 class="tasks-title">Tarefas</h1>
        <span *ngIf="!loading && tasks.length > 0" class="tasks-count">
          {{ tasks.length }} {{ tasks.length === 1 ? 'tarefa' : 'tarefas' }}
        </span>
      </div>

      <section class="tasks-new">
        <h2 class="tasks-section-title">Nova tarefa</h2>
        <form [formGroup]="createForm" (ngSubmit)="createTask()" class="tasks-form">
          <div class="tasks-field">
            <label class="field-label" for="title">Título</label>
            <input class="field-input" id="title" type="text" formControlName="title" placeholder="Título da tarefa" />
            <app-error-message
              *ngIf="createForm.controls.title.invalid && createForm.controls.title.touched"
              message="Título é obrigatório.">
            </app-error-message>
          </div>

          <div class="tasks-field">
            <label class="field-label" for="description">Descrição</label>
            <textarea
              class="field-input field-textarea"
              id="description"
              rows="2"
              formControlName="description"
              placeholder="Descreva a tarefa (opcional)"
            ></textarea>
          </div>

          <div class="tasks-form-footer">
            <app-button type="submit" [disabled]="isSubmitting">
              {{ isSubmitting ? 'Salvando...' : 'Criar tarefa' }}
            </app-button>
          </div>

          <app-error-message *ngIf="submitErrorMessage" [message]="submitErrorMessage"></app-error-message>
        </form>
      </section>

      <section class="tasks-list-section">
        <app-loading [visible]="loading" text="Carregando tarefas..."></app-loading>
        <app-error-message *ngIf="errorMessage" [message]="errorMessage"></app-error-message>
        <app-empty-state
          [visible]="!loading && !errorMessage && tasks.length === 0"
          message="Nenhuma tarefa encontrada. Crie sua primeira tarefa acima.">
        </app-empty-state>

        <div *ngIf="!loading && tasks.length > 0" class="tasks-grid">
          <div *ngFor="let task of tasks" class="task-card" [ngClass]="'task-card--' + (selectedStatuses[task.id] ?? task.status)?.toLowerCase()">
            <div class="task-card__top">
              <span class="task-badge" [ngClass]="'task-badge--' + (selectedStatuses[task.id] ?? task.status)?.toLowerCase()">
                {{ statusLabel(selectedStatuses[task.id] ?? task.status) }}
              </span>
              <small *ngIf="updatingTaskId === task.id" class="task-updating">Atualizando...</small>
            </div>

            <strong class="task-card__title">{{ task.title }}</strong>
            <p class="task-card__desc">{{ task.description || 'Sem descrição.' }}</p>

            <div class="task-card__status">
              <label class="field-label" [for]="'status-' + task.id">Alterar status:</label>
              <div class="task-status-row">
                <select
                  class="task-select"
                  [id]="'status-' + task.id"
                  [value]="selectedStatuses[task.id] ?? task.status"
                  (change)="setSelectedStatus(task.id, getSelectValue($event))"
                  [disabled]="updatingTaskId === task.id"
                >
                  <option value="Todo">A fazer</option>
                  <option value="Doing">Em andamento</option>
                  <option value="Done">Concluído</option>
                </select>
                <app-button
                  type="button"
                  (clicked)="saveTaskStatus(task)"
                  [disabled]="updatingTaskId === task.id"
                >
                  Salvar
                </app-button>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>
  `,
  styles: [
    `
      .tasks-container {
        max-width: 680px;
      }

      .tasks-back {
        display: inline-block;
        font-size: 0.875rem;
        color: var(--mat-sys-primary, #1976d2);
        text-decoration: none;
        margin-bottom: 16px;
      }

      .tasks-back:hover {
        text-decoration: underline;
      }

      .tasks-header {
        display: flex;
        align-items: baseline;
        gap: 12px;
        margin-bottom: 24px;
      }

      .tasks-title {
        margin: 0;
      }

      .tasks-count {
        font-size: 0.875rem;
        color: var(--mat-sys-on-surface-variant, #757575);
      }

      .tasks-section-title {
        font-size: 0.8rem;
        font-weight: 500;
        margin: 0 0 12px;
        color: var(--mat-sys-on-surface-variant, #555);
        text-transform: uppercase;
        letter-spacing: 0.06em;
      }

      .tasks-new {
        background: #fff;
        border-radius: 8px;
        box-shadow: 0 1px 4px rgba(0,0,0,0.08);
        padding: 20px 24px;
        margin-bottom: 32px;
      }

      .tasks-form {
        display: flex;
        flex-direction: column;
        gap: 14px;
      }

      .tasks-field {
        display: flex;
        flex-direction: column;
        gap: 6px;
      }

      .field-label {
        font-size: 0.875rem;
        font-weight: 500;
        color: var(--mat-sys-on-surface-variant, #555);
      }

      .field-input {
        padding: 10px 12px;
        border: 1px solid #bdbdbd;
        border-radius: 4px;
        font-size: 1rem;
        outline: none;
        transition: border-color 0.15s;
        box-sizing: border-box;
        width: 100%;
        font-family: inherit;
      }

      .field-input:focus {
        border-color: var(--mat-sys-primary, #1976d2);
        box-shadow: 0 0 0 2px rgba(25, 118, 210, 0.15);
      }

      .field-textarea {
        resize: vertical;
        min-height: 64px;
      }

      .tasks-form-footer {
        display: flex;
        justify-content: flex-end;
      }

      .tasks-list-section {
        display: flex;
        flex-direction: column;
        gap: 12px;
      }

      .tasks-grid {
        display: flex;
        flex-direction: column;
        gap: 12px;
      }

      .task-card {
        background: #fff;
        border-radius: 8px;
        box-shadow: 0 1px 3px rgba(0,0,0,0.08);
        padding: 16px 20px;
        display: flex;
        flex-direction: column;
        gap: 8px;
        border-left: 4px solid #e0e0e0;
        transition: box-shadow 0.15s;
      }

      .task-card--todo { border-left-color: #9e9e9e; }
      .task-card--doing { border-left-color: var(--mat-sys-primary, #1976d2); }
      .task-card--done { border-left-color: #2e7d32; }

      .task-card__top {
        display: flex;
        align-items: center;
        gap: 10px;
      }

      .task-badge {
        display: inline-block;
        padding: 2px 10px;
        border-radius: 12px;
        font-size: 0.75rem;
        font-weight: 500;
        letter-spacing: 0.03em;
      }

      .task-badge--todo {
        background: #f5f5f5;
        color: #616161;
      }

      .task-badge--doing {
        background: #e3f2fd;
        color: #1565c0;
      }

      .task-badge--done {
        background: #e8f5e9;
        color: #2e7d32;
      }

      .task-updating {
        color: var(--mat-sys-on-surface-variant, #757575);
        font-style: italic;
      }

      .task-card__title {
        font-size: 1rem;
        font-weight: 600;
        color: var(--mat-sys-on-surface, #212121);
      }

      .task-card__desc {
        font-size: 0.875rem;
        color: var(--mat-sys-on-surface-variant, #757575);
        margin: 0;
        line-height: 1.4;
      }

      .task-card__status {
        margin-top: 4px;
        display: flex;
        flex-direction: column;
        gap: 6px;
      }

      .task-status-row {
        display: flex;
        align-items: center;
        gap: 8px;
      }

      .task-select {
        padding: 6px 10px;
        border: 1px solid #bdbdbd;
        border-radius: 4px;
        font-size: 0.875rem;
        font-family: inherit;
        outline: none;
        cursor: pointer;
        background: #fff;
      }

      .task-select:focus {
        border-color: var(--mat-sys-primary, #1976d2);
      }

      .task-select:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }
    `,
  ],
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

  selectedStatuses: Record<string, string | undefined> = {};

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

  getSelectValue(event: Event): string {
    return (event.target as HTMLSelectElement).value;
  }

  setSelectedStatus(taskId: string, status: string): void {
    this.selectedStatuses[taskId] = status;
  }

  statusLabel(status: string | undefined): string {
    switch (status) {
      case 'Todo': return 'A fazer';
      case 'Doing': return 'Em andamento';
      case 'Done': return 'Concluído';
      default: return status ?? '';
    }
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