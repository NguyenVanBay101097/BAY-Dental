import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeKeepingSettingDialogComponent } from './time-keeping-setting-dialog.component';

describe('TimeKeepingSettingDialogComponent', () => {
  let component: TimeKeepingSettingDialogComponent;
  let fixture: ComponentFixture<TimeKeepingSettingDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TimeKeepingSettingDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeKeepingSettingDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
