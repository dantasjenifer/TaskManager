import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../services/task';
import { TaskItem } from '../../models/task-model';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-list.html',
  styleUrls: ['./task-list.css']
})
export class TaskList implements OnInit {
  tasks: TaskItem[] = [];
  newTaskTitle: string = '';
  newTaskDescription: string = '';
  errorMessage: string = '';
  isLoading: boolean = false;

  constructor(private taskService: TaskService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.taskService.getTasks().subscribe({
      next: (data) => {
        this.tasks = data;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Error loading tasks. Please check your connection and try again.';
        this.isLoading = false;
      }
    });
  }

  addTask(): void {
    if (!this.newTaskTitle.trim()) {
      this.errorMessage = 'Please enter a task title.';
      this.cdr.detectChanges();
      return;
    }

    const newTask: TaskItem = { 
      title: this.newTaskTitle.trim(),
      description: this.newTaskDescription.trim() || undefined
    };

    this.taskService.createTask(newTask).subscribe({
      next: (createdTask) => {
        this.tasks.push(createdTask);
        this.newTaskTitle = '';
        this.newTaskDescription = '';
        this.errorMessage = '';
        this.cdr.detectChanges();
      },
      error: () => this.errorMessage = 'Error creating task. Please check your connection and try again.'
    });
  }

  completeTask(id: string): void {
    this.errorMessage = '';
    this.taskService.completeTask(id).subscribe({
      next: () => {
        this.tasks = this.tasks.map(task => 
          task.id === id ? { ...task, status: 1 } : task
        );
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Error completing task. This task may already be completed.';
      }
    });
  }

  deleteTask(id: string): void {
    this.errorMessage = '';
    this.taskService.deleteTask(id).subscribe({
      next: () => {
        this.tasks = this.tasks.filter(task => task.id !== id);
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Error deleting task. This task may not exist.';
      }
    });
  }
}