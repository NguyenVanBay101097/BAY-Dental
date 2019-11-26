import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportLocationComponent } from './partner-report-location.component';

describe('PartnerReportLocationComponent', () => {
  let component: PartnerReportLocationComponent;
  let fixture: ComponentFixture<PartnerReportLocationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportLocationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportLocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
