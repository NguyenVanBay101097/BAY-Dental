import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceCalendarAttendanceCreateUpdateDialogComponent } from './resource-calendar-attendance-create-update-dialog.component';

describe('ResourceCalendarAttendanceCreateUpdateDialogComponent', () => {
  let component: ResourceCalendarAttendanceCreateUpdateDialogComponent;
  let fixture: ComponentFixture<ResourceCalendarAttendanceCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceCalendarAttendanceCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceCalendarAttendanceCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
