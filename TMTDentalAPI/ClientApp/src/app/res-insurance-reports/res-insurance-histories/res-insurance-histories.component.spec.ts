import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceHistoriesComponent } from './res-insurance-histories.component';

describe('ResInsuranceHistoriesComponent', () => {
  let component: ResInsuranceHistoriesComponent;
  let fixture: ComponentFixture<ResInsuranceHistoriesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceHistoriesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceHistoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
