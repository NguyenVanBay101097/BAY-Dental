import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentKanbanComponent } from './appointment-kanban.component';

describe('AppointmentKanbanComponent', () => {
  let component: AppointmentKanbanComponent;
  let fixture: ComponentFixture<AppointmentKanbanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentKanbanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentKanbanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
