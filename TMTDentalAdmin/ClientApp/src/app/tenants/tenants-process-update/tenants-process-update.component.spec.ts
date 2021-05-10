import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantsProcessUpdateComponent } from './tenants-process-update.component';

describe('TenantsProcessUpdateComponent', () => {
  let component: TenantsProcessUpdateComponent;
  let fixture: ComponentFixture<TenantsProcessUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantsProcessUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantsProcessUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
