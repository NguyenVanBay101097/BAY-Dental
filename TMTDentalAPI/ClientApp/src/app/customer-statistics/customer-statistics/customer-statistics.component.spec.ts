import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerStatisticsComponent } from './customer-statistics.component';

describe('CustomerStatisticsComponent', () => {
  let component: CustomerStatisticsComponent;
  let fixture: ComponentFixture<CustomerStatisticsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerStatisticsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerStatisticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
