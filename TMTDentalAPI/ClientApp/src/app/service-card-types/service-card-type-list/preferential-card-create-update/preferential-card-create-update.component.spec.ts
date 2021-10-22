import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PreferentialCardCreateUpdateComponent } from './preferential-card-create-update.component';

describe('PreferentialCardCreateUpdateComponent', () => {
  let component: PreferentialCardCreateUpdateComponent;
  let fixture: ComponentFixture<PreferentialCardCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PreferentialCardCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PreferentialCardCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
