import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceReportsOverviewComponent } from './res-insurance-reports-overview.component';

describe('ResInsuranceReportsOverviewComponent', () => {
  let component: ResInsuranceReportsOverviewComponent;
  let fixture: ComponentFixture<ResInsuranceReportsOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceReportsOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceReportsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
