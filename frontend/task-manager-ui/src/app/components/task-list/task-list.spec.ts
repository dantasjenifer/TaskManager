import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TaskList } from './task-list';
import { TaskService } from '../../services/task';
import { TaskItem } from '../../models/task-model';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { vi } from 'vitest';

describe('TaskList', () => {
  let component: TaskList;
  let fixture: ComponentFixture<TaskList>;
  let taskServiceSpy: { getTasks: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    const spy = {
      getTasks: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [TaskList, CommonModule],
      providers: [
        { provide: TaskService, useValue: spy }
      ]
    }).compileComponents();

    taskServiceSpy = TestBed.inject(TaskService) as unknown as { getTasks: ReturnType<typeof vi.fn> };
  });

  it('should create', async () => {
    taskServiceSpy.getTasks.mockReturnValue(of([]));
    
    fixture = TestBed.createComponent(TaskList);
    component = fixture.componentInstance;
    await fixture.whenStable();
    
    expect(component).toBeTruthy();
  });

  it('should load tasks on initialization', async () => {
    const mockTasks: TaskItem[] = [
      { id: '1', title: 'Task Test 1', status: 0 },
      { id: '2', title: 'Task Test 2', status: 1 }
    ];
    taskServiceSpy.getTasks.mockReturnValue(of(mockTasks));

    fixture = TestBed.createComponent(TaskList);
    component = fixture.componentInstance;
    await fixture.whenStable();

    expect(taskServiceSpy.getTasks).toHaveBeenCalled();
    expect(component.tasks.length).toBe(2);
    expect(component.tasks[0].title).toBe('Task Test 1');
  });

  it('should handle error when loading tasks fails', async () => {
    taskServiceSpy.getTasks.mockReturnValue(throwError(() => new Error('API Error')));

    fixture = TestBed.createComponent(TaskList);
    component = fixture.componentInstance;
    await fixture.whenStable();

    expect(component.errorMessage).toBe('Error loading tasks from API.');
  });
});