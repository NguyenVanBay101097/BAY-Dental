import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeKeepingForallDialogComponent } from './time-keeping-forall-dialog.component';

describe('TimeKeepingForallDialogComponent', () => {
  let component: TimeKeepingForallDialogComponent;
  let fixture: ComponentFixture<TimeKeepingForallDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TimeKeepingForallDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeKeepingForallDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
