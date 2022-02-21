import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportSourceComponent } from './partner-report-source.component';

describe('PartnerReportSourceComponent', () => {
  let component: PartnerReportSourceComponent;
  let fixture: ComponentFixture<PartnerReportSourceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportSourceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportSourceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
