import { Routes } from '@angular/router';
import { HomeComponent } from './features/home.component';
import { LoginComponent } from './features/auth/login/login.component';
import { ProjectsComponent } from './features/projects/projects.component';
import { TasksComponent } from './features/tasks/tasks.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    { 
        path: '', 
        component: HomeComponent 
    },
    { 
        path: 'login', 
        component: LoginComponent 
    },
    { 
        path: 'projects', 
        component: ProjectsComponent ,
        canActivate: [authGuard],
    },
    { 
        path: 'projects/:projectId/tasks', 
        component: TasksComponent,
        canActivate: [authGuard],
    },
];
