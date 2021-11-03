import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardCardsPreferentialComponent } from './service-card-cards-preferential.component';

describe('ServiceCardCardsPreferentialComponent', () => {
  let component: ServiceCardCardsPreferentialComponent;
  let fixture: ComponentFixture<ServiceCardCardsPreferentialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardCardsPreferentialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardCardsPreferentialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
