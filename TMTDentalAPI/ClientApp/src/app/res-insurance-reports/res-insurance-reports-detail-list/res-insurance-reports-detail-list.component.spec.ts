import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceReportsDetailListComponent } from './res-insurance-reports-detail-list.component';

describe('ResInsuranceReportsDetailListComponent', () => {
  let component: ResInsuranceReportsDetailListComponent;
  let fixture: ComponentFixture<ResInsuranceReportsDetailListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceReportsDetailListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceReportsDetailListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
