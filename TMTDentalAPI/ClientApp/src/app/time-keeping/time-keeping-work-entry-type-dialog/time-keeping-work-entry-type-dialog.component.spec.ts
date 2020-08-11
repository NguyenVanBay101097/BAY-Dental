import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeKeepingWorkEntryTypeDialogComponent } from './time-keeping-work-entry-type-dialog.component';

describe('TimeKeepingWorkEntryTypeDialogComponent', () => {
  let component: TimeKeepingWorkEntryTypeDialogComponent;
  let fixture: ComponentFixture<TimeKeepingWorkEntryTypeDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TimeKeepingWorkEntryTypeDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeKeepingWorkEntryTypeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
