import { NgFor, NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectsService } from '../../core/services/projects.service';
import { Project } from './project.model';
import { RouterLink } from '@angular/router';
import { ErrorMessageComponent } from '../../shared/components/error-message/error-message.component';
import { AppButtonComponent } from '../../shared/components/app-button/app-button.component';
import { AppLoadingComponent } from '../../shared/components/app-loading/app-loading.component';
import { AppEmptyStateComponent } from '../../shared/components/app-empty-state/app-empty-state.component';
@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule, RouterLink, ErrorMessageComponent, AppButtonComponent, AppLoadingComponent, AppEmptyStateComponent],
  template: `
    <main class="projects-container">
      <div class="projects-header">
        <h1 class="projects-title">Meus Projetos</h1>
        <span *ngIf="!loading && projects.length > 0" class="projects-count">
          {{ projects.length }} {{ projects.length === 1 ? 'projeto' : 'projetos' }}
        </span>
      </div>

      <section class="projects-new">
        <h2 class="projects-section-title">Novo projeto</h2>
        <form [formGroup]="form" (ngSubmit)="createProject()" class="projects-form">
          <div class="projects-field">
            <label class="field-label" for="name">Nome</label>
            <input class="field-input" id="name" type="text" formControlName="name" placeholder="Nome do projeto" />
            <app-error-message
              *ngIf="form.controls.name.touched && form.controls.name.hasError('required')"
              message="Nome é obrigatório.">
            </app-error-message>
          </div>

          <div class="projects-field">
            <label class="field-label" for="description">Descrição</label>
            <textarea
              class="field-input field-textarea"
              id="description"
              formControlName="description"
              rows="2"
              placeholder="Descreva o projeto (opcional)"
            ></textarea>
          </div>

          <div class="projects-form-footer">
            <app-button type="submit" [disabled]="form.invalid || saving">
              {{ saving ? 'Salvando...' : 'Criar projeto' }}
            </app-button>
          </div>

          <app-error-message *ngIf="saveErrorMessage" [message]="saveErrorMessage"></app-error-message>
        </form>
      </section>

      <section class="projects-list-section">
        <app-loading [visible]="loading" text="Carregando projetos..."></app-loading>
        <app-error-message *ngIf="errorMessage" [message]="errorMessage"></app-error-message>
        <app-empty-state
          [visible]="!loading && !errorMessage && projects.length === 0"
          message="Nenhum projeto encontrado. Crie seu primeiro projeto acima.">
        </app-empty-state>

        <div *ngIf="!loading && projects.length > 0" class="projects-grid">
          <a
            *ngFor="let project of projects"
            [routerLink]="['/projects', project.id, 'tasks']"
            class="project-card"
          >
            <span class="project-card__icon">📋</span>
            <div class="project-card__body">
              <span class="project-card__name">{{ project.name }}</span>
              <span class="project-card__desc">{{ project.description || 'Sem descrição.' }}</span>
            </div>
            <span class="project-card__arrow">›</span>
          </a>
        </div>
      </section>
    </main>
  `,
  styles: [
    `
      .projects-container {
        max-width: 680px;
      }

      .projects-header {
        display: flex;
        align-items: baseline;
        gap: 12px;
        margin-bottom: 24px;
      }

      .projects-title {
        margin: 0;
      }

      .projects-count {
        font-size: 0.875rem;
        color: var(--mat-sys-on-surface-variant, #757575);
      }

      .projects-section-title {
        font-size: 1rem;
        font-weight: 500;
        margin: 0 0 12px;
        color: var(--mat-sys-on-surface-variant, #555);
        text-transform: uppercase;
        letter-spacing: 0.06em;
        font-size: 0.8rem;
      }

      .projects-new {
        background: #fff;
        border-radius: 8px;
        box-shadow: 0 1px 4px rgba(0,0,0,0.08);
        padding: 20px 24px;
        margin-bottom: 32px;
      }

      .projects-form {
        display: flex;
        flex-direction: column;
        gap: 14px;
      }

      .projects-field {
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
        border-color: var(--mat-sys-primary, #3949ab);
        box-shadow: 0 0 0 2px rgba(57, 73, 171, 0.15);
      }

      .field-textarea {
        resize: vertical;
        min-height: 64px;
      }

      .projects-form-footer {
        display: flex;
        justify-content: flex-end;
      }

      .projects-list-section {
        display: flex;
        flex-direction: column;
        gap: 12px;
      }

      .projects-grid {
        display: flex;
        flex-direction: column;
        gap: 10px;
      }

      .project-card {
        display: flex;
        align-items: center;
        gap: 16px;
        background: #fff;
        border-radius: 8px;
        box-shadow: 0 1px 3px rgba(0,0,0,0.08);
        padding: 16px 20px;
        text-decoration: none;
        color: inherit;
        transition: box-shadow 0.15s, transform 0.1s;
        border: 1px solid transparent;
      }

      .project-card:hover {
        box-shadow: 0 3px 8px rgba(0,0,0,0.14);
        border-color: var(--mat-sys-primary, #3949ab);
        transform: translateY(-1px);
      }

      .project-card__icon {
        font-size: 1.4rem;
        flex-shrink: 0;
      }

      .project-card__body {
        flex: 1;
        display: flex;
        flex-direction: column;
        gap: 4px;
        min-width: 0;
      }

      .project-card__name {
        font-weight: 600;
        font-size: 1rem;
        color: var(--mat-sys-on-surface, #212121);
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }

      .project-card__desc {
        font-size: 0.875rem;
        color: var(--mat-sys-on-surface-variant, #757575);
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }

      .project-card__arrow {
        font-size: 1.4rem;
        color: var(--mat-sys-on-surface-variant, #bdbdbd);
        flex-shrink: 0;
      }
    `,
  ],
})

export class ProjectsComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);
  private readonly fb = inject(FormBuilder);

  loading = true;
  saving = false;
  errorMessage = '';
  saveErrorMessage = '';
  projects: Project[] = [];

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required]],
    description: [''],
  });

  ngOnInit(): void {
    this.loadProjects();
  }

  createProject(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.saveErrorMessage = '';

    const formValue = this.form.getRawValue();

    this.projectsService
      .create({
        name: formValue.name.trim(),
        description: formValue.description.trim() || null,
      })
      .subscribe({
        next: (project) => {
          this.projects = [...this.projects, project].sort((a, b) =>
            a.name.localeCompare(b.name),
          );

          this.form.reset({ name: '', description: '' });
          this.saving = false;
        },
        error: () => {
          this.saveErrorMessage = 'Erro ao criar projeto.';
          this.saving = false;
        },
      });
  }

  private loadProjects(): void {
    this.loading = true;
    this.errorMessage = '';

    this.projectsService.getAll().subscribe({
      next: (projects) => {
        this.projects = projects;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar projetos.';
        this.loading = false;
      },
    });
  }

}