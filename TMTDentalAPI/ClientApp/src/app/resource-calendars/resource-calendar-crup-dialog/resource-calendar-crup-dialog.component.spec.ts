import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceCalendarCrupDialogComponent } from './resource-calendar-crup-dialog.component';

describe('ResourceCalendarCrupDialogComponent', () => {
  let component: ResourceCalendarCrupDialogComponent;
  let fixture: ComponentFixture<ResourceCalendarCrupDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceCalendarCrupDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceCalendarCrupDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
