import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookMassMessagingScheduleDialogComponent } from './facebook-mass-messaging-schedule-dialog.component';

describe('FacebookMassMessagingScheduleDialogComponent', () => {
  let component: FacebookMassMessagingScheduleDialogComponent;
  let fixture: ComponentFixture<FacebookMassMessagingScheduleDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookMassMessagingScheduleDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookMassMessagingScheduleDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
