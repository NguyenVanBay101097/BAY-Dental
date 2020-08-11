import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeKeepingWorkEntryTypeComponent } from './time-keeping-work-entry-type.component';

describe('TimeKeepingWorkEntryTypeComponent', () => {
  let component: TimeKeepingWorkEntryTypeComponent;
  let fixture: ComponentFixture<TimeKeepingWorkEntryTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TimeKeepingWorkEntryTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeKeepingWorkEntryTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
