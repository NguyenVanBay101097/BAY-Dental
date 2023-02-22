import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DotKhamStepReportComponent } from './dot-kham-step-report.component';

describe('DotKhamStepReportComponent', () => {
  let component: DotKhamStepReportComponent;
  let fixture: ComponentFixture<DotKhamStepReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DotKhamStepReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DotKhamStepReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
