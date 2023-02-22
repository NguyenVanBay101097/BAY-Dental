import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeKeepingSetupDialogComponent } from './time-keeping-setup-dialog.component';

describe('TimeKeepingSetupDialogComponent', () => {
  let component: TimeKeepingSetupDialogComponent;
  let fixture: ComponentFixture<TimeKeepingSetupDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TimeKeepingSetupDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeKeepingSetupDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
