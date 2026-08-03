import { Component, OnInit } from '@angular/core';
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

  constructor(private taskService: TaskService) {}

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
        this.errorMessage = 'Error loading tasks from API.';
        this.isLoading = false;
      }
    });
  }

  addTask(): void {
    if (!this.newTaskTitle.trim()) return;

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
      },
      error: () => this.errorMessage = 'Error creating task.'
    });
  }
}