import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoutingCreateUpdateComponent } from './routing-create-update.component';

describe('RoutingCreateUpdateComponent', () => {
  let component: RoutingCreateUpdateComponent;
  let fixture: ComponentFixture<RoutingCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoutingCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoutingCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
