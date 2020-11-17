import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeKeepingPopoverComponent } from './time-keeping-popover.component';

describe('TimeKeepingPopoverComponent', () => {
  let component: TimeKeepingPopoverComponent;
  let fixture: ComponentFixture<TimeKeepingPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TimeKeepingPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeKeepingPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
