import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskList } from './components/task-list/task-list';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, TaskList],
  template: `
    <main>
      <app-task-list></app-task-list>
    </main>
  `,
  styleUrls: ['./app.css']
})
export class App {
  title = 'task-manager-ui';
}