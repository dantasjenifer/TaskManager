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
  let taskServiceSpy: { 
    getTasks: ReturnType<typeof vi.fn>; 
    createTask: ReturnType<typeof vi.fn>;
    updateTask: ReturnType<typeof vi.fn>;
    deleteTask: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    const spy = {
      getTasks: vi.fn(),
      createTask: vi.fn(),
      updateTask: vi.fn(),
      deleteTask: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [TaskList, CommonModule],
      providers: [
        { provide: TaskService, useValue: spy }
      ]
    }).compileComponents();

    taskServiceSpy = TestBed.inject(TaskService) as unknown as typeof spy;
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

  it('should add a new task successfully', async () => {
    taskServiceSpy.getTasks.mockReturnValue(of([]));
    const newTask: TaskItem = { title: 'Brand New Task' };
    const savedTask: TaskItem = { id: 'guid-123', title: 'Brand New Task', status: 0 };
    
    taskServiceSpy.createTask.mockReturnValue(of(savedTask));

    fixture = TestBed.createComponent(TaskList);
    component = fixture.componentInstance;
    await fixture.whenStable();

    component.newTaskTitle = 'Brand New Task';
    component.addTask();

    expect(taskServiceSpy.createTask).toHaveBeenCalledWith({ title: 'Brand New Task' });
    expect(component.tasks.length).toBe(1);
    expect(component.tasks[0].title).toBe('Brand New Task');
    expect(component.newTaskTitle).toBe('');
  });

  it('should complete/update task status successfully', async () => {
    const mockTasks: TaskItem[] = [
      { id: 'guid-1', title: 'Task to Complete'}
    ];
    taskServiceSpy.getTasks.mockReturnValue(of(mockTasks));
    taskServiceSpy.completeTask.mockReturnValue(of(void 0));

    fixture = TestBed.createComponent(TaskList);
    component = fixture.componentInstance;
    await fixture.whenStable();

    // Se o seu método no componente se chamar completeTask ou updateTask, ajuste aqui:
    component.completeTask(mockTasks[0]);

    expect(taskServiceSpy.completeTask).toHaveBeenCalledWith('guid-1');
  });

  it('should delete a task successfully', async () => {
    const mockTasks: TaskItem[] = [
      { id: 'guid-1', title: 'Task to Delete'}
    ];
    taskServiceSpy.getTasks.mockReturnValue(of(mockTasks));
    taskServiceSpy.deleteTask.mockReturnValue(of(void 0));

    fixture = TestBed.createComponent(TaskList);
    component = fixture.componentInstance;
    await fixture.whenStable();

    component.deleteTask('guid-1');

    expect(taskServiceSpy.deleteTask).toHaveBeenCalledWith('guid-1');
  });
});