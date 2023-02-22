import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeKeepingImportFileComponent } from './time-keeping-import-file.component';

describe('TimeKeepingImportFileComponent', () => {
  let component: TimeKeepingImportFileComponent;
  let fixture: ComponentFixture<TimeKeepingImportFileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TimeKeepingImportFileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeKeepingImportFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
