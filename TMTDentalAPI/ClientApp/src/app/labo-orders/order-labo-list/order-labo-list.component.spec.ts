import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderLaboListComponent } from './order-labo-list.component';

describe('OrderLaboListComponent', () => {
  let component: OrderLaboListComponent;
  let fixture: ComponentFixture<OrderLaboListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrderLaboListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderLaboListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
