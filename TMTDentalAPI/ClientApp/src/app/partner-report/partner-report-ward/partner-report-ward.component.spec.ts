import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportWardComponent } from './partner-report-ward.component';

describe('PartnerReportWardComponent', () => {
  let component: PartnerReportWardComponent;
  let fixture: ComponentFixture<PartnerReportWardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportWardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportWardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
