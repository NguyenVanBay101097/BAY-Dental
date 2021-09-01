import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerReceipCreateUpdateComponent } from './customer-receip-create-update.component';

describe('CustomerReceipCreateUpdateComponent', () => {
  let component: CustomerReceipCreateUpdateComponent;
  let fixture: ComponentFixture<CustomerReceipCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerReceipCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerReceipCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
