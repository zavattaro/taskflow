import { NgFor, NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectsService } from '../../core/services/projects.service';
import { Project } from './project.model';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AppHeaderComponent } from '../../shared/components/app-header/app-header.component';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule, RouterLink, AppHeaderComponent],
  template: `
  <app-header></app-header>
    <main style="padding: 24px; font-family: Arial, sans-serif; max-width: 700px;">
      <p style="margin-bottom: 16px;">
        <button type="button" (click)="logout()" style="padding: 8px 12px;">
          Sair
        </button>
      </p>
      <h1>Projetos</h1>

      <form
        [formGroup]="form"
        (ngSubmit)="createProject()"
        style="display: flex; flex-direction: column; gap: 12px; margin-bottom: 24px;"
      >
        <div>
          <label for="name">Nome</label>
          <input
            id="name"
            type="text"
            formControlName="name"
            style="width: 100%; padding: 8px; box-sizing: border-box;"
          />

          <small
            *ngIf="form.controls.name.touched && form.controls.name.hasError('required')"
            style="color: red;"
          >
            Nome é obrigatório.
          </small>
        </div>

        <div>
          <label for="description">Descrição</label>
          <textarea
            id="description"
            formControlName="description"
            rows="3"
            style="width: 100%; padding: 8px; box-sizing: border-box;"
          ></textarea>
        </div>

        <button type="submit" [disabled]="form.invalid || saving" style="padding: 10px; width: 180px;">
          {{ saving ? 'Salvando...' : 'Criar projeto' }}
        </button>

        <p *ngIf="saveErrorMessage" style="color: red; margin: 0;">
          {{ saveErrorMessage }}
        </p>
      </form>

      <p *ngIf="loading">Carregando projetos...</p>

      <p *ngIf="errorMessage" style="color: red;">
        {{ errorMessage }}
      </p>

      <p *ngIf="!loading && !errorMessage && projects.length === 0">
        Nenhum projeto encontrado.
      </p>

      <ul *ngIf="!loading && projects.length > 0" style="padding-left: 20px;">
        <li *ngFor="let project of projects" style="margin-bottom: 12px;">
            <a
                [routerLink]="['/projects', project.id, 'tasks']"
                style="font-weight: bold; text-decoration: none; color: #0d6efd;"
            >
                {{ project.name }}
            </a>
            <br />
            <span>{{ project.description || 'Sem descrição.' }}</span>
        </li>
    </ul>
    </main>
  `,
})
export class ProjectsComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);

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

    this.projectsService.create({
      name: formValue.name.trim(),
      description: formValue.description.trim() || null,
    }).subscribe({
      next: (project) => {
        this.projects = [...this.projects, project].sort((a, b) =>
          a.name.localeCompare(b.name),
        );

        this.form.reset({
          name: '',
          description: '',
        });

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

   logout(): void {
    this.authService.logout();
  }
}